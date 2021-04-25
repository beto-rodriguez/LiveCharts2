using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Design.LinearGradients
{
    public class ViewModel
    {
        public ViewModel()
        {
            var colors = new[]
            {
                new SKColor(45, 64, 89),
                new SKColor(234, 84, 85),
                new SKColor(240, 123, 63),
                new SKColor(255, 212, 96)
            };

            Series = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Values = new []{ 3, 7, 2, 9, 4 },
                    Stroke = null,
                    Fill = new LinearGradientPaintTask(
                        new [] {new SKColor(255, 140, 148), new SKColor(220, 237, 194) },
                        new SKPoint(0.5f, 0),
                        new SKPoint(0.5f, 1))
                },
                new LineSeries<int>
                {
                    Values = new []{ 4, 2, 8, 5, 3 },
                    GeometrySize = 22,
                    Stroke = new LinearGradientPaintTask(colors) { StrokeThickness = 10 },
                    GeometryStroke = new LinearGradientPaintTask(colors) { StrokeThickness = 10 },
                    Fill = null
                }
            };
        }

        public ISeries[] Series { get; set; }
    }
}
