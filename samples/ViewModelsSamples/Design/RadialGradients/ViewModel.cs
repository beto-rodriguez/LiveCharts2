using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Design.RadialGradients
{
    public class ViewModel
    {
        public ViewModel()
        {
            Series = new ISeries[]
            {
                new PieSeries<int>
                {
                    Values = new []{ 7 },
                    Stroke = null,
                    Fill = new RadialGradientPaintTask(new SKColor(179, 229, 252), new SKColor(1, 87, 155)),
                    MaxOuterRadius = 0.8
                },
                new PieSeries<int>
                {
                    Values = new []{ 3 },
                    Stroke = null,
                    Fill = new RadialGradientPaintTask(new SKColor(255, 205, 210), new SKColor(183, 28, 28))
                }
            };
        }

        public ISeries[] Series { get; set; }
    }
}
