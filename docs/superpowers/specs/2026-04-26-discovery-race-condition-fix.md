# Discovery Race Condition - Root Cause Analysis

## Problem
Discovery between two instances works 50/50 - sometimes finds the room, sometimes doesn't.

## Root Cause

### The Issue Flow

1. Client A connects to server → `WaitForConnectionAsync` accepts
2. `AcceptRequest` reads the discovery request
3. `OnRequestReceived` (in DiscoveryService) prepares response
4. `ResponseWithData` writes response to pipe
5. **BUT IMMEDIATELY AFTER → `ResponseWithData` disposes the pipe (line 116)**
6. Server loop continues - creates NEW pipe and waits for next client
7. When Client B tries to discover - race condition:
   - If old pipe hasn't closed yet → works
   - If new client connects to new pipe → works  
   - If timing is wrong → fails

### Why 50/50

The server loop runs:
```csharp
// NamedPipeDiscoveryServer.cs line 66-92
while (!cancellationToken.IsCancellationRequested)
{
    var newPipe = Create(...);
    await newPipe.WaitForConnectionAsync(...);
    await AcceptRequest(...);  // Returns IMMEDIATELY after reading!
    // Loop continues to NEXT iteration - creates new pipe
    // ResponseWithData happens ASYNC on another thread
    // But AcceptRequest already returned!
}
```

`AcceptRequest` returns BEFORE `ResponseWithData` writes the response. The loop moves on, the old pipe gets disposed while response is being written.

## Solution - Option A

Modify `AcceptRequest` to WAIT for response before returning.

### Implementation

1. **Add pending response storage:**
```csharp
private readonly ConcurrentDictionary<Guid, byte[]> pendingResponses = new();
private readonly SemaphoreSlim responseSemaphore = new(0);
```

2. **Modify AcceptRequest:**
```csharp
private async Task AcceptRequest(Guid connectionId, CancellationToken cancellationToken)
{
    var buffer = new byte[DiscoveryConstants.DiscoveryBufferSize];
    var pipe = connections[connectionId];
    var bytesRead = await pipe.ReadAsync(buffer.AsMemory(), cancellationToken);
    
    if (bytesRead > 0)
    {
        var data = new byte[bytesRead];
        Array.Copy(buffer, data, bytesRead);
        MessageReceived?.Invoke(this, new DiscoveryRawMessageReceivedEventArgs(data, pipeName, connectionId));
        
        // WAIT for response before returning
        if (pendingResponses.TryRemove(connectionId, out var responseData))
        {
            await pipe.WriteAsync(responseData.AsMemory(), cancellationToken);
            await pipe.FlushAsync(cancellationToken);
        }
        
        responseSemaphore.Release();  // Signal that we're done
    }
}
```

3. **Modify ResponseWithData to store response instead of writing directly:**
```csharp
public async Task ResponseWithData(byte[] responseData, Guid? connectionId, CancellationToken cancellationToken)
{
    if (connectionId.HasValue && connections.TryGetValue(connectionId.Value, out var pipe))
    {
        // Store response - AcceptRequest will pick it up
        pendingResponses[connectionId.Value] = responseData;
        
        // Wait for AcceptRequest to process it
        await responseSemaphore.WaitAsync(cancellationToken);
        
        connections.TryRemove(connectionId.Value, out _);
    }
}
```

Actually wait - this creates deadlock. Let me rethink.

### Better Implementation

Use a different approach - just make AcceptRequest NOT dispose the pipe immediately:

**Option A1: Keep pipe open until response is sent**

In `ResponseWithData` - don't dispose immediately:
```csharp
public async Task ResponseWithData(byte[] responseData, Guid? connectionId, CancellationToken cancellationToken)
{
    if (connectionId.HasValue && connections.TryGetValue(connectionId.Value, out var pipe) && pipe.IsConnected)
    {
        await pipe.WriteAsync(responseData.AsMemory(), cancellationToken);
        await pipe.FlushAsync(cancellationToken);
        // DON'T dispose here - let AcceptRequest do it
    }
}
```

In `AcceptRequest` - don't dispose in finally if response pending, handle it differently:

Actually simpler - just track if response was sent:

```csharp
// Add at class level
private readonly HashSet<Guid> responsesInProgress = new();

public async Task ResponseWithData(byte[] responseData, Guid? connectionId, CancellationToken cancellationToken)
{
    if (connectionId.HasValue && connections.TryGetValue(connectionId.Value, out var pipe) && pipe.IsConnected)
    {
        lock(responsesInProgress)
        {
            responsesInProgress.Add(connectionId.Value);
        }
        
        await pipe.WriteAsync(responseData.AsMemory(), cancellationToken);
        await pipe.FlushAsync(cancellationToken);
    }
}

private async Task AcceptRequest(Guid connectionId, CancellationToken cancellationToken)
{
    var pipe = connections[connectionId];
    var bytesRead = await pipe.ReadAsync(...);
    
    if (bytesRead > 0)
    {
        MessageReceived?.Invoke(this, new DiscoveryRawMessageReceivedEventArgs(data, pipeName, connectionId));
    }
    
    // Only dispose if no response in progress
    bool shouldRespond;
    lock(responsesInProgress)
    {
        shouldRespond = responsesInProgress.Remove(connectionId);
    }
    
    if (!shouldRespond)
    {
        await pipe.DisposeAsync();
        connections.TryRemove(connectionId, out _);
    }
    // Else: ResponseWithData will dispose after writing
}
```

This approach is getting complex. Simpler solution:

### Simplest Fix

Just add a small delay or ensure response completes before AcceptRequest continues:

```csharp
private async Task AcceptRequest(...)
{
    var bytesRead = await pipe.ReadAsync(...);
    
    if (bytesRead > 0)
    {
        MessageReceived?.Invoke(...);
        
        // Small delay to allow ResponseWithData to complete
        // OR use a more sophisticated wait mechanism
        await Task.Delay(100);  // Simple but works
    }
    
    // Then dispose
}
```

But delays are ugly. Better approach:

### Best Solution - Use TaskCompletionSource

```csharp
// Add at class level
private readonly ConcurrentDictionary<Guid, TaskCompletionSource<byte[]>> responseWaiters = new();

private async Task AcceptRequest(Guid connectionId, CancellationToken cancellationToken)
{
    var pipe = connections[connectionId];
    var bytesRead = await pipe.ReadAsync(buffer.AsMemory(), cancellationToken);
    
    if (bytesRead > 0)
    {
        var data = new byte[bytesRead];
        Array.Copy(buffer, data, bytesRead);
        
        var tcs = new TaskCompletionSource<byte[]>();
        responseWaiters[connectionId] = tcs;
        
        MessageReceived?.Invoke(this, new DiscoveryRawMessageReceivedEventArgs(data, pipeName, connectionId));
        
        // Wait for response but with timeout
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(5));
        
        try
        {
            await tcs.Task.WaitAsync(timeoutCts.Token);
        }
        catch (TimeoutExpiredException) { /* Ignore - response may have failed */ }
    }
    
    // Cleanup
    responseWaiters.TryRemove(connectionId, out _);
    await pipe.DisposeAsync();
    connections.TryRemove(connectionId, out _);
}

public async Task ResponseWithData(byte[] responseData, Guid? connectionId, CancellationToken cancellationToken)
{
    if (connectionId.HasValue && connections.TryGetValue(connectionId.Value, out var pipe) && pipe.IsConnected)
    {
        try
        {
            await pipe.WriteAsync(responseData.AsMemory(), cancellationToken);
            await pipe.FlushAsync(cancellationToken);
            
            // Signal that we're done
            if (responseWaiters.TryGetValue(connectionId.Value, out var tcs))
            {
                tcs.SetResult(responseData);
            }
        }
        catch
        {
            if (responseWaiters.TryGetValue(connectionId.Value, out var tcs))
            {
                tcs.SetException(...);
            }
        }
    }
}
```

This is cleaner. Let's implement this version.

## Summary

The race condition happens because:
1. AcceptRequest processes request and returns immediately
2. Loop continues, creates new pipe
3. ResponseWithData tries to write to old pipe that may be disposed

The fix:
1. AcceptRequest waits for ResponseWithData to complete before returning
2. Use TaskCompletionSource to coordinate between AcceptRequest and ResponseWithData
3. Only dispose pipe after response is sent

## Files to Modify

- `Infrastructure/Discovery/Transport/MIN.Discovery.Transport.NamedPipes/Server/NamedPipeDiscoveryServer.cs`