using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ViewModelsSamples.Scatter.BackImage
{
    public class ViewModel
    {
        public IEnumerable<ISeries> Series { get; set; } = new ObservableCollection<ISeries>
        {
            new ScatterSeries<ObservablePoint>
            {
                Values = new ObservableCollection<ObservablePoint>
                {
                    new ObservablePoint(0.3, 0.3),
                    new ObservablePoint(0.4, 0.4),
                    new ObservablePoint(0.5, 0.5),
                    new ObservablePoint(0.6, 0.6),
                    new ObservablePoint(0.7, 0.7),
                }
            }
        };

        public DrawMarginFrame DrawMarginFrame { get; set; } = new DrawMarginFrame();

        public ViewModel()
        {

            DrawMarginFrame.BackImage = new BackImagePaintTask();
            var backImagePaintTask = (BackImagePaintTask)DrawMarginFrame.BackImage;
            var dirPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var filePath = System.IO.Path.Combine(dirPath, @"Scatter\BackImage", "CIE1975.png");
            //var filePath = System.IO.Path.Combine(dirPath, @"Scatter\BackImage", "wood.png");
            //var filePath = System.IO.Path.Combine(dirPath, @"Scatter\BackImage", "family.png");
            var bitmap = SkiaSharp.SKBitmap.Decode(filePath);

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

            backImagePaintTask.BackImage = new BackgroundImage(imgFormat, bitmap.Bytes, bitmap.Width, bitmap.Height, 70)
            {
                //MappingWithData.
                //The image is mapped to X:0~0.7f, Y:0~0.7 coordinates.
                RenderMode = BackgroundImage.ImageRenderMode.MappingWithData,
                //If you choose MappingWithData, you shoud set mapping information.
                ImageMapping = new ImageMapping(new System.Drawing.PointF(0, 0.7f), new System.Drawing.PointF(0.7f, 0)),

                //Keep aspect ratio.
                //RenderMode = BackgroundImage.ImageRenderMode.KeepAspectRatio,

                //Stretch image to the chart space.
                //RenderMode = BackgroundImage.ImageRenderMode.Stretch,
            };
        }
    }
}
