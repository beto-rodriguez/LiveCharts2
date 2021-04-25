using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Design.StrokeDashArray
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
                new LineSeries<int>
                {
                    Values = new [] { 4, 2, 8, 5, 3 },
                    LineSmoothness = 1,
                    GeometrySize = 22,
                    Stroke = new SolidColorPaintTask() {
                        StrokeCap = SKStrokeCap.Round,
                        StrokeThickness = 10,

                    },
                    GeometryStroke = new LinearGradientPaintTask(colors) { StrokeThickness = 10 },
                    Fill = null
                }
            };
        }

        public ISeries[] Series { get; set; }
    }
}
