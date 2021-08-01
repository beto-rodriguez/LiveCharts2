using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System;

namespace ViewModelsSamples.Axes.Shared
{
    public class ViewModel
    {
        public ViewModel()
        {
            var values = new int[100];
            var values2 = new int[100];
            var r = new Random();
            var t = 0;
            var t2 = 0;

            for (var i = 0; i < 100; i++)
            {
                t += r.Next(-90, 100);
                values[i] = t;

                t2 += r.Next(-90, 100);
                values2[i] = t2;
            }

            SeriesCollection1 = new ISeries[] { new LineSeries<int> { Values = values } };
            SeriesCollection2 = new ISeries[] { new ColumnSeries<int> { Values = values2 } };

            // sharing the same instance for both charts will keep the zooming and panning synced
            SharedXAxis = new Axis[] { new Axis() };
        }

        public ISeries[] SeriesCollection1 { get; set; }

        public ISeries[] SeriesCollection2 { get; set; }

        public Axis[] SharedXAxis { get; set; }
    }
}
