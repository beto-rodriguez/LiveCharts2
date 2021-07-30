using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ViewModelsSamples.Scatter.Basic
{
    public class ViewModel
    {
        public IEnumerable<ISeries> Series { get; set; } = new ObservableCollection<ISeries>
        {
            new ScatterSeries<ObservablePoint>
            {
                Values = new ObservableCollection<ObservablePoint>
                {
                    new ObservablePoint(0, 0),
                    new ObservablePoint(0.3, 0.3),
                    new ObservablePoint(0.4, 0.4),
                    new ObservablePoint(0.5, 0.5),
                    new ObservablePoint(0.6, 0.6),
                    new ObservablePoint(0.7, 0.7),

                    //new ObservablePoint(2.2, 5.4),
                    //new ObservablePoint(4.5, 2.5),
                    //new ObservablePoint(4.2, 7.4),
                    //new ObservablePoint(6.4, 9.9),
                    //new ObservablePoint(4.2, 9.2),
                    //new ObservablePoint(5.8, 3.5),
                    //new ObservablePoint(7.3, 5.8),
                    //new ObservablePoint(8.9, 3.9),
                    //new ObservablePoint(6.1, 4.6),
                    //new ObservablePoint(9.4, 7.7),
                    //new ObservablePoint(8.4, 8.5),
                    //new ObservablePoint(3.6, 9.6),
                    //new ObservablePoint(4.4, 6.3),
                    //new ObservablePoint(5.8, 4.8),
                    //new ObservablePoint(6.9, 3.4),
                    //new ObservablePoint(7.6, 1.8),
                    //new ObservablePoint(8.3, 8.3),
                    //new ObservablePoint(9.9, 5.2),
                    //new ObservablePoint(8.1, 4.7),
                    //new ObservablePoint(7.4, 3.9),
                    //new ObservablePoint(6.8, 2.3),
                    //new ObservablePoint(5.3, 7.1),
                }
            }
        };

        public DrawMarginFrame DrawMarginFrame { get; set; } = new DrawMarginFrame();

        public ViewModel()
        {
            DrawMarginFrame.BackImage = new BackImagePaintTask();
            var backImagePaintTask = (BackImagePaintTask)DrawMarginFrame.BackImage;
            var bitmap = SkiaSharp.SKBitmap.Decode("D:\\CIE1975.png");
            var imgFormat = BackgroundImage.ImageFormat.RGB8;
#pragma warning disable IDE0010 // Add missing cases
            switch (bitmap.ColorType)
#pragma warning restore IDE0010 // Add missing cases
            {
                case SkiaSharp.SKColorType.Rgb888x:
                    imgFormat = BackgroundImage.ImageFormat.RGB8;
                    break;
                case SkiaSharp.SKColorType.Rgba8888:
                    imgFormat = BackgroundImage.ImageFormat.RGBA8;
                    break;
                case SkiaSharp.SKColorType.Bgra8888:
                    imgFormat = BackgroundImage.ImageFormat.BGRA8;
                    break;
                case SkiaSharp.SKColorType.Gray8:
                    imgFormat = BackgroundImage.ImageFormat.Mono8;
                    break;
                default:
                    //not support
                    Debug.Assert(false);
                    break;
            }
            backImagePaintTask.BackImage = new BackgroundImage(imgFormat, bitmap.Bytes, bitmap.Width, bitmap.Height, 30)
            {
                RenderMode = BackgroundImage.ImageRenderMode.MappingWithData,
                ImageMapping = new ImageMapping(new System.Drawing.PointF(0, 0.7f), new System.Drawing.PointF(0.7f, 0))
            };
        }
    }
}
