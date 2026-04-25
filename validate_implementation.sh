#!/usr/bin/env bash
# Simple validation script to check the implementation approach

echo "=== Validating RecentRoomCard Navigation Implementation ==="
echo ""

# Check that RecentRoomCard has the necessary structure
echo "1. Checking RecentRoomCard structure..."
if grep -q "public event Func<Task>? Clicked" /mnt/c/Users/Admin/Documents/CSharpProjects/Learning/Projects/MIN/Desktop/MIN.Desktop/Views/Components/RecentRoomCard.cs; then
    echo "✓ RecentRoomCard has Clicked event"
else
    echo "✗ RecentRoomCard missing Clicked event"
fi

if grep -q "SubscribeToEvents" /mnt/c/Users/Admin/Documents/CSharpProjects/Learning/Projects/MIN/Desktop/MIN.Desktop/Views/Components/RecentRoomCard.cs; then
    echo "✓ RecentRoomCard subscribes to events"
else
    echo "✗ RecentRoomCard missing event subscription"
fi

if grep -q "OnRoomJoined" /mnt/c/Users/Admin/Documents/CSharpProjects/Learning/Projects/MIN/Desktop/MIN.Desktop/Views/Components/RecentRoomCard.cs; then
    echo "✓ RecentRoomCard has RoomJoinedEvent handler"
else
    echo "✗ RecentRoomCard missing RoomJoinedEvent handler"
fi

echo ""
echo "2. Checking MainSidePanelView structure..."
if grep -q "CreateRecentRoomCard" /mnt/c/Users/Admin/Documents/CSharpProjects/Learning/Projects/MIN/Desktop/MIN.Desktop/Views/Panels/SidePanelViews/MainSidePanelView.cs; then
    echo "✓ MainSidePanelView has CreateRecentRoomCard method"
else
    echo "✗ MainSidePanelView missing CreateRecentRoomCard method"
fi

if grep -q "NavigateToRoomChat" /mnt/c/Users/Admin/Documents/CSharpProjects/Learning/Projects/MIN/Desktop/MIN.Desktop/Views/Panels/SidePanelViews/MainSidePanelView.cs; then
    echo "✓ MainSidePanelView has NavigateToRoomChat method"
else
    echo "✗ MainSidePanelView missing NavigateToRoomChat method"
fi

if grep -q "Controls.Add.*flowLayoutPanelRooms" /mnt/c/Users/Admin/Documents/CSharpProjects/Learning/Projects/MIN/Desktop/MIN.Desktop/Views/Panels/SidePanelViews/MainSidePanelView.cs; then
    echo "✓ MainSidePanelView uses flowLayoutPanelRooms (via designer)"
else
    echo "✗ MainSidePanelView missing flowLayoutPanelRooms usage"
fi

echo ""
echo "3. Checking NavigationService usage..."
if grep -q "navigationService.NavigateTo<ChatPanelView" /mnt/c/Users/Admin/Documents/CSharpProjects/Learning/Projects/MIN/Desktop/MIN.Desktop/Views/Panels/SidePanelViews/MainSidePanelView.cs; then
    echo "✓ NavigationService used to navigate to ChatPanelView"
else
    echo "✗ NavigationService usage not found"
fi

echo ""
echo "4. Checking ChatPanelView registration..."
if grep -q "RegisterAsImplementedInterfaces<ChatPanelView>(ServiceLifetime.Singleton)" /mnt/c/Users/Admin/Documents/CSharpProjects/Learning/Projects/MIN/Desktop/MIN.Desktop/Program.cs; then
    echo "✓ ChatPanelView registered as Singleton"
else
    echo "✗ ChatPanelView registration not found"
fi

echo ""
echo "5. Checking RoomJoinedEvent usage..."
if grep -q "RoomJoinedEvent" /mnt/c/Users/Admin/Documents/CSharpProjects/Learning/Projects/MIN/Desktop/MIN.Desktop/Views/Components/RecentRoomCard.cs; then
    echo "✓ RecentRoomCard subscribes to RoomJoinedEvent"
else
    echo "✗ RoomJoinedEvent subscription not found"
fi

if grep -q "OnRoomJoined" /mnt/c/Users/Admin/Documents/CSharpProjects/Learning/Projects/MIN/Desktop/MIN.Desktop/Views/Components/RecentRoomCard.cs; then
    echo "✓ RecentRoomCard has OnRoomJoined handler"
else
    echo "✗ OnRoomJoined handler not found"
fi

echo ""
echo "=== Validation Complete ==="