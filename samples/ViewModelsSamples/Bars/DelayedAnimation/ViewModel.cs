using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace ViewModelsSamples.Bars.DelayedAnimation;

public partial class ViewModel : ObservableObject
{
    public ViewModel()
    {
        var values1 = new List<float>();
        var values2 = new List<float>();

        var fx = EasingFunctions.BounceInOut; // this is the function we are going to plot
        var x = 0f;

        while (x <= 1)
        {
            values1.Add(fx(x));
            values2.Add(fx(x - 0.15f));
            x += 0.025f;
        }

        var columnSeries1 = new ColumnSeries<float>
        {
            Values = values1,
            Stroke = null,
            Padding = 2
        };

        var columnSeries2 = new ColumnSeries<float>
        {
            Values = values2,
            Stroke = null,
            Padding = 2
        };

        columnSeries1.PointMeasured += OnPointMeasured;
        columnSeries2.PointMeasured += OnPointMeasured;

        Series = new List<ISeries> { columnSeries1, columnSeries2 };
    }

    private void OnPointMeasured(ChartPoint<float, RoundedRectangleGeometry, LabelGeometry> point)
    {
        var perPointDelay = 100; // milliseconds
        var delay = point.Context.Entity.MetaData!.EntityIndex * perPointDelay;
        var speed = (float)point.Context.Chart.AnimationsSpeed.TotalMilliseconds + delay;

        point.Visual?.SetTransition(
            new Animation(progress =>
            {
                var d = delay / speed;

                return progress <= d
                    ? 0
                    : EasingFunctions.BuildCustomElasticOut(1.5f, 0.60f)((progress - d) / (1 - d));
            },
            TimeSpan.FromMilliseconds(speed)));
    }

    public List<ISeries> Series { get; set; }
}
