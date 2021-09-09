
namespace ExportGanttToPDF
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.IO;
    using Syncfusion.Windows.Controls.Gantt;
    using Microsoft.Win32;
    using Syncfusion.Pdf.Graphics;
    using Syncfusion.Pdf;
    using System.Drawing;
    using Syncfusion.Windows.Controls.Gantt.Grid;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        private Grid layoutgrid;
        private GanttGrid ganttGrid;
        private ScrollViewer chart;

        #endregion

        #region Contructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Handles the Click event of the ExportBtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ExportBtn_Click(object sender, RoutedEventArgs e)
        {
            Border child = VisualTreeHelper.GetChild(Gantt, 0) as Border;
            this.layoutgrid = VisualTreeHelper.GetChild(child, 0) as Grid;

            // To get the gantt grid from visual tree
            this.ganttGrid = VisualTreeHelper.GetChild(layoutgrid, 0) as GanttGrid;

            // To get the gantt chart from visual tree
            this.chart = Gantt.FindName<ScrollViewer>("PART_ScheduleViewScrollViewer");
            this.ExportGanttToPDF();
        }

        /// <summary>
        /// To export Gantt as PDF.
        /// </summary>
        private void ExportGanttToPDF()
        {
            this.layoutgrid.Children.Remove(this.ganttGrid);
            Canvas InnerCanvas = new Canvas();
            InnerCanvas.Children.Add(this.ganttGrid);
            InnerCanvas.Width = 300;
            InnerCanvas.Height = 500;
            System.Drawing.Size size = new System.Drawing.Size((int)InnerCanvas.ActualWidth, (int)InnerCanvas.ActualHeight);
            InnerCanvas.Arrange(new Rect(0, 0, size.Width, size.Height));
            InnerCanvas.UpdateLayout();
            this.ganttGrid.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
            this.Gantt.Arrange(new Rect(new System.Windows.Size(this.ganttGrid.ActualWidth, this.ganttGrid.ActualHeight)));
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)this.ganttGrid.ActualWidth, (int)this.ganttGrid.ActualHeight, 96, 96, PixelFormats.Default);
            renderTargetBitmap.Render(this.ganttGrid);
            JpegBitmapEncoder jpegBitmapEncoder = new JpegBitmapEncoder();
            jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
            FileStream stream = new FileStream("grid.jpg",
            FileMode.OpenOrCreate, FileAccess.Write);
            jpegBitmapEncoder.Save(stream);
            stream.Close();
            InnerCanvas.Children.Remove(this.ganttGrid);
            this.layoutgrid.Children.Add(this.ganttGrid);

            this.Gantt.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
            this.Gantt.Arrange(new Rect(new System.Windows.Size(this.Gantt.DesiredSize.Width, this.Gantt.DesiredSize.Height)));
            RenderTargetBitmap bmp = new RenderTargetBitmap(
                   (int)this.Gantt.ActualWidth, (int)this.Gantt.ActualHeight, 96, 96, PixelFormats.Default);
            bmp.Render(chart);
            int width = (int)(this.Gantt.DesiredSize.Width - chart.ActualWidth);
            CroppedBitmap crpBmp = new CroppedBitmap(bmp, new Int32Rect(width, 0, (int)chart.ActualWidth, (int)chart.ActualHeight));
            PngBitmapEncoder bitmapEncoder = new PngBitmapEncoder();
            bitmapEncoder.Frames.Add(BitmapFrame.Create(crpBmp));
            FileStream chartstream = new FileStream("chart.jpg",
            FileMode.OpenOrCreate, FileAccess.Write);
            bitmapEncoder.Save(chartstream);
            chartstream.Close();

            //// Gets the bitmap frame from the images.
            BitmapFrame frame1 = BitmapDecoder.Create(new Uri("grid.jpg", UriKind.Relative), BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames.First();
            BitmapFrame frame2 = BitmapFrame.Create(new Uri("chart.jpg", UriKind.Relative),
                                      BitmapCreateOptions.None,
                                      BitmapCacheOption.OnLoad);

            // Draws the images into a DrawingVisual component
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawImage(frame1, new Rect(0, 0, frame1.PixelWidth, frame1.PixelHeight));
                drawingContext.DrawImage(frame2, new Rect(frame1.PixelWidth, 0, frame2.PixelWidth, frame2.PixelHeight));
            }

            // Converts the Visual (DrawingVisual) into a BitmapSource
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)frame1.PixelWidth + frame2.PixelWidth, (int)frame2.PixelHeight, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(drawingVisual);
            var bitmapEncoder1 = new PngBitmapEncoder();
            bitmapEncoder1.Frames.Add(BitmapFrame.Create(bitmap));

            // Creates a PngBitmapEncoder and adds the BitmapSource to the frames of the encoder
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "Untited";
            saveFileDialog.DefaultExt = "PDF";
            saveFileDialog.FilterIndex = 1;

            //Saving the images in Pdf           
            PdfDocument doc = new PdfDocument();
            doc.PageSettings = new PdfPageSettings(new SizeF(3000, 4000));
            PdfPage page = doc.Pages.Add();
            PdfGraphics graphics = page.Graphics;
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
        }

        #endregion
    }
}