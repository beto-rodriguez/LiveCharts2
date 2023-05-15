using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Easing;
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
        var visual = point.Visual;
        if (visual is null) return;

        var baseFunction = EasingFunctions.BuildCustomElasticOut(1.5f, 0.60f);

        visual.Animate(
            normalizedTime =>
            {
                var d = (float)(point.Context.Entity.EntityIndex / point.Context.Chart.AnimationsSpeed.TotalMilliseconds);
                if (normalizedTime <= d) return 0;

                var delayedProgress = (normalizedTime - d) / (1 - d);
                return baseFunction(delayedProgress);
            },
            point.Context.Chart.AnimationsSpeed);
    }

    public List<ISeries> Series { get; set; }
}
