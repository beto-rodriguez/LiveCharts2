using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using System.Collections.Generic;

namespace ViewModelsSamples.Pies.Gauge
{
    public class ViewModel
    {
        public ViewModel()
        {
            var ir = 0;

            GaugeTotal = 60;

            Series = new List<ISeries>
            {
                new PieSeries<double>
                {
                    Values = new List<double> { 0 },
                    Fill = new SolidColorPaintTask(new SkiaSharp.SKColor(220, 220, 220, 90)),
                    InnerRadius = ir,
                    //RelativeInnerRadius = 30,
                    //RelativeOuterRadius = 30,
                    IsFillSeries = true
                },
                new PieSeries<double> { Values = new List<double> { 20 }, InnerRadius = ir, RelativeInnerRadius = 0 }
            };
        }

        public IEnumerable<ISeries> Series { get; set; }

        public double GaugeTotal { get; set; }
    }
}
