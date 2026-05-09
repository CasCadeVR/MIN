# Chat Image Preview Message Card Design

## Problem
`ChatImagePreviewMessageCard` was a carbon copy of `ChatFileMessageCard` with no actual image preview functionality.

## Solution
Transform it into a proper image preview card with:
- `ImageHelper` utility for safe image loading/scaling
- Dynamic sizing based on image aspect ratio (80-85% of parent width)
- Three states: placeholder (not downloaded) → downloading → image preview (downloaded)

## Components

### 1. `ImageHelper` (new: `Desktop/MIN.Desktop/Infrastructure/ImageHelper.cs`)
Static utility class:
- `GetDimensions(string filePath) → (int Width, int Height)` — uses `Image.FromStream` to avoid file-locking
- `LoadScaled(string filePath, int maxWidth) → Image` — returns pre-scaled Bitmap maintaining aspect ratio using `HighQualityBicubic` interpolation

### 2. `ChatImagePreviewMessageCard` (rewritten)
**Designer layout:** `Heading3Label fileNameAndSize` (centered, fills Panel1 of `splitContainerDownload`) + `ProgressBar downloadProgressBar` in Panel2.

**States:**
| State | fileNameAndSize | Progress bar | Click action |
|---|---|---|---|
| Not downloaded | Text: "FileName FileSize", BackgroundImage: download icon | Hidden | Fire `OnDownloadRequested` |
| Downloading | Text: "FileName X / Y", BackgroundImage: close icon | Visible | Fire `OnCancelRequested` |
| Downloaded | Image: scaled preview, BackgroundImage: null, Text: "" | Hidden | No-op |

**`ResizeOutOfPrefferedSize()`:**
- If not downloaded → return current Height
- `maxWidth = Parent!.Width * 0.85`
- `(imgW, imgH) = ImageHelper.GetDimensions(filePath)`
- `ratio = imgW / imgH`
- `Width = Math.Min(maxWidth, imgW)`
- `imageHeight = (int)(Width / ratio)`
- `Height = headerRowHeight(if !removeHeaders) + imageHeight`
- Load and assign: `fileNameAndSize.Image = ImageHelper.LoadScaled(filePath, Width)`
- Return `Height`

**`OnFileTransferCompleted`:**
- Set `downloaded = true`, `filePath = eventMessage.FilePath`
- `fileNameAndSize.BackgroundImage = null`, `Text = ""`
- `splitContainerDownload.Panel2Collapsed = true`
- Call `ResizeOutOfPrefferedSize()`

### 3. `ChatPanelView.Messages.cs` change
After `row.container.Controls.Add(rowControl)`:
```csharp
if (rowControl is ChatImagePreviewMessageCard imageCard)
{
    row.Height = imageCard.ResizeOutOfPrefferedSize() + row.Padding.Top;
}
```
