# How-to-export-the-Gantt-grid-and-chart-as-PDF-in-WPF-GanttControl
This article explains how to export the Syncfusion WPF Gantt chart and grid (combined) to the PDF.

![](Output.png)

This can be achieved by getting the Gantt grid and chart from the visual tree and arrange it in the canvas to convert the as bitmap and save as image. Please refer the following code snippet.

**Step 1**: Initialize the Gantt control and create a Model and ViewModel class then create a list of collections in the ViewModel class and add it into ItemSource.

[XAML]
```
<sync:GanttControl x:Name="Gantt"
                   ItemsSource="{Binding TaskCollection}"
                   ResourceCollection="{Binding ResourceCollection}">
```

**Step 2**: Get the Gantt grid and chart from the visual tree and arrange it in the canvas as like in below code sample.

[C#]
```
…
System.Windows.Controls.Canvas InnerCanvas = new System.Windows.Controls.Canvas();
InnerCanvas.Children.Add(Ganttgrid);
InnerCanvas.Width = 300;
InnerCanvas.Height = 500;
System.Drawing.Size size = new System.Drawing.Size((int)InnerCanvas.ActualWidth, (int)InnerCanvas.ActualHeight);
InnerCanvas.Arrange(new Rect(0, 0, size.Width, size.Height));
InnerCanvas.UpdateLayout();
Ganttgrid.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
Gantt.Arrange(new Rect(new System.Windows.Size(Ganttgrid.ActualWidth, Ganttgrid.ActualHeight)));
RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)Ganttgrid.ActualWidth, (int)Ganttgrid.ActualHeight, 96,96, PixelFormats.Default);
renderTargetBitmap.Render(Ganttgrid);
JpegBitmapEncoder jpegBitmapEncoder = new JpegBitmapEncoder();
jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
FileStream stream = new FileStream("grid.jpg",
FileMode.OpenOrCreate, FileAccess.Write);
jpegBitmapEncoder.Save(stream);
stream.Close();
InnerCanvas.Children.Remove(Ganttgrid);
Layoutgrid.Children.Add(Ganttgrid);
Gantt.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
Gantt.Arrange(new Rect(new System.Windows.Size(Gantt.DesiredSize.Width, Gantt.DesiredSize.Height)));
…
```

**Step 3**: After arranged in canvas, convert the as bitmap and save as image.

[C#]
```
…
RenderTargetBitmap bmp = new RenderTargetBitmap((int)Gantt.ActualWidth, (int)Gantt.ActualHeight, 96, 96, PixelFormats.Default;
bmp.Render(chart);
int width = (int)(Gantt.DesiredSize.Width - chart.ActualWidth);
CroppedBitmap crpBmp = new CroppedBitmap(bmp, new Int32Rect(width, 0,(int)chart.ActualWidth, (int)chart.ActualHeight));
PngBitmapEncoder bitmapEncoder = new PngBitmapEncoder();
bitmapEncoder.Frames.Add(BitmapFrame.Create(crpBmp));
FileStream chartstream = new FileStream("chart.jpg",
FileMode.OpenOrCreate, FileAccess.Write);
bitmapEncoder.Save(chartstream);
chartstream.Close();
…
```

**Step 4**: Now convert to PDF using the image saved stream as like in the below code snippet.

[C#]
```
…
//Saving the images in Pdf           
PdfDocument doc = new PdfDocument();
doc.PageSettings = new PdfPageSettings(new SizeF(3000, 4000));
PdfPage page = doc.Pages.Add();
PdfGraphics graphics = page.Graphics;
PdfPage page1 = doc.Pages.Add();
if (saveFileDialog.ShowDialog() == true)
{
    using (Stream saveStream = saveFileDialog.OpenFile())
    {
        encoder.Save(saveStream);
        bitmapEncoder1.Save(saveStream);
        saveStream.Seek(0, SeekOrigin.Begin);
        PdfBitmap image1 = new PdfBitmap(saveStream);
        graphics.DrawImage(image1, 0, 0);
        saveStream.Close();
    }
}

doc.Save(saveFileDialog.FileName);
doc.Close(true);
…

```

## See also

[How to add custom tooltip to Gantt](https://help.syncfusion.com/wpf/gantt/customtooltip)

[How to define your own schedule for Gantt to track the progress of projects](https://help.syncfusion.com/wpf/gantt/custom-schedule)

[How to differentiate the dates of holidays](https://help.syncfusion.com/wpf/gantt/holidays-customization)