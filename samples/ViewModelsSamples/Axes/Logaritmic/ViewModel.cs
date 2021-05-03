using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ViewModelsSamples.Axes.Logaritmic
{
    public class ViewModel
    {
        // base 10 log, change the base if you require it.
        // or use any custom scale the logic is the same.
        private static readonly int s_logBase = 10;

        public ViewModel()
        {
            // you must normally call this when the application starts.
            LiveCharts.Configure(config =>
                config.HasMap<LogaritmicPoint>((model, point) =>
                {
                    point.SecondaryValue = (float)model.X;
                    point.PrimaryValue = (float)Math.Log(model.Y, s_logBase);
                }));
        }

        public IEnumerable<Axis> YAxes { get; set; } = new Axis[]
        {
            new Axis
            {
                MinStep = 1, // forces the step of the axis to be at least 1
                Labeler = value => Math.Pow(s_logBase, value).ToString() // converts the log scale back for the label
            }
        };

        public IEnumerable<ISeries> Series { get; set; } = new ObservableCollection<ISeries>
        {
            new LineSeries<LogaritmicPoint>
            {
                Values = new ObservableCollection<LogaritmicPoint>
                {
                    new LogaritmicPoint
                    {
                        X = 1,
                        Y = 10
                    },
                    new LogaritmicPoint
                    {
                        X = 2,
                        Y = 100
                    },
                    new LogaritmicPoint
                    {
                        X = 3,
                        Y = 1000
                    },
                    new LogaritmicPoint
                    {
                        X = 4,
                        Y = 10000
                    },
                    new LogaritmicPoint
                    {
                        X = 5,
                        Y = 100000
                    },
                    new LogaritmicPoint
                    {
                        X = 6,
                        Y = 1000000
                    },
                    new LogaritmicPoint
                    {
                        X = 7,
                        Y = 10000000
                    }
                }
            }
        };
    }

    public class LogaritmicPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
    }
}
