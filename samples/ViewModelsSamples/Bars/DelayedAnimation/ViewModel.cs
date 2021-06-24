using LiveChartsCore;
using LiveChartsCore.Easing;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using System.Collections.Generic;

namespace ViewModelsSamples.Bars.DelayedAnimation
{
    public class ViewModel
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
                Stroke = null
            };

            var columnSeries2 = new ColumnSeries<float>
            {
                Values = values2,
                Stroke = null
            };

            columnSeries1.PointMeasured += OnPointMeasured;
            columnSeries2.PointMeasured += OnPointMeasured;

            Series = new List<ISeries> { columnSeries1, columnSeries2 };
        }

        private void OnPointMeasured(TypedChartPoint<float, RoundedRectangleGeometry, LabelGeometry, SkiaSharpDrawingContext> point)
        {
            var visual = point.Visual;
            var delayedFunction = new DelayedFunction(EasingFunctions.BuildCustomElasticOut(1.5f, 0.60f), point, 30f);

            _ = visual
                .TransitionateProperties(
                    nameof(visual.Y),
                    nameof(visual.Height))
                .WithAnimation(animation =>
                    animation
                        .WithDuration(delayedFunction.Speed)
                        .WithEasingFunction(delayedFunction.Function));
        }

        public List<ISeries> Series { get; set; }
    }
}
