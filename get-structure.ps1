# TO EXECUTE WRITE .\get-structure.ps1 IN COMMAND PROMPT

function Show-Tree {
    param(
        [string]$Path = ".",
        [string]$Indent = ""
    )
    
    $result = @()
    $items = Get-ChildItem -Path $Path | Where-Object { $_.Name -notin @('bin', 'obj', '.vs', 'packages', 'node_modules') } | Sort-Object Name
    
    for ($i = 0; $i -lt $items.Count; $i++) {
        $item = $items[$i]
        $isLast = ($i -eq $items.Count - 1)
        
        if ($isLast) {
            $result += "$Indent+-- $($item.Name)"
            $newIndent = "$Indent    "
        } else {
            $result += "$Indent+-- $($item.Name)"
            $newIndent = "$Indent|   "
        }
        
        if ($item.PSIsContainer) {
            $result += Show-Tree -Path $item.FullName -Indent $newIndent
        }
    }
    
    return $result
}

# Get current directory
$currentPath = Get-Location
$root = Split-Path -Leaf $currentPath

# Get tree structure as array of lines
$treeLines = Show-Tree -Path $currentPath

# Create output with proper line breaks
$output = @($root) + $treeLines

# Save to file - each array element becomes a new line
$output | Out-File -FilePath "structure.txt" -Encoding ASCII

Write-Host "Done! Structure saved to structure.txt" -ForegroundColor Green