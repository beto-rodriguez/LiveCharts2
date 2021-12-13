using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Axes.Logaritmic;

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
            new()
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
                    new()
                    {
                        X = 1,
                        Y = 10
                    },
                    new()
                    {
                        X = 2,
                        Y = 100
                    },
                    new()
                    {
                        X = 3,
                        Y = 1000
                    },
                    new()
                    {
                        X = 4,
                        Y = 10000
                    },
                    new()
                    {
                        X = 5,
                        Y = 100000
                    },
                    new()
                    {
                        X = 6,
                        Y = 1000000
                    },
                    new()
                    {
                        X = 7,
                        Y = 10000000
                    }
                }
            }
        };
}
