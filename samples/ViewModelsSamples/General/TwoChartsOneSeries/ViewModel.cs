using System;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.General.TwoChartsOneSeries
{
    public class ViewModel
    {
        public ViewModel()
        {
            var values = new int[100];
            var r = new Random();
            var t = 0;

            for (var i = 0; i < 100; i++)
            {
                t += r.Next(-90, 100);
                values[i] = t;
            }

            Series = new ISeries[] { new StepLineSeries<int> { Values = values } };
        }

        public ISeries[] Series { get; set; }
    }
}
