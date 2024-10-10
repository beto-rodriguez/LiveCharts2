using System;
using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace ViewModelsSamples.Bars.DelayedAnimation;

public class ViewModel
{
    public List<ISeries> Series { get; set; }

    public ViewModel()
    {
        var columnSeries1 = new ColumnSeries<float>
        {
            Values = FetchVales(0),
            Stroke = null,
            Padding = 2
        };

        var columnSeries2 = new ColumnSeries<float>
        {
            Values = FetchVales(-0.15f),
            Stroke = null,
            Padding = 2
        };

        columnSeries1.PointMeasured += OnPointMeasured;
        columnSeries2.PointMeasured += OnPointMeasured;

        Series = [columnSeries1, columnSeries2];
    }

    private void OnPointMeasured(ChartPoint<float, RoundedRectangleGeometry, LabelGeometry> point)
    {
        var perPointDelay = 100; // in milliseconds
        var delay = point.Context.Entity.MetaData!.EntityIndex * perPointDelay;
        var speed = (float)point.Context.Chart.AnimationsSpeed.TotalMilliseconds + delay;

        // the animation takes a function, that represents the progress of the animation
        // the parameter is the progress of the animation, it goes from 0 to 1
        // the function must return a value from 0 to 1, where 0 is the initial state
        // and 1 is the end state

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

    private static List<float> FetchVales(float offset)
    {
        var values = new List<float>();

        // the EasingFunctions.BounceInOut, is just
        // a function that takes a double and returns a double

        var fx = EasingFunctions.BounceInOut;
        var x = 0f;

        while (x <= 1)
        {
            values.Add(fx(x + offset));
            x += 0.025f;
        }

        return values;
    }
}
