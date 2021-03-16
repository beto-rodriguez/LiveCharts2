using LiveChartsCore;
using LiveChartsCore.Context;
using LiveChartsCore.Easing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;

namespace ViewModelsSamples.Bars.DelayedAnimation
{
    public class ViewModel
    {
        public ViewModel()
        {
            var values1 = new List<float>();
            var values2 = new List<float>();

            var x = 0f;
            while(x <= 1)
            {
                var y = EasingFunctions.BounceInOut(x);
                values1.Add(y);
                values2.Add(y);
                x += 0.01f;
            }

            var columnSeries1 = new ColumnSeries<float>
            { 
                Values = values1,
                Stroke = null,
                Fill = new SolidColorPaintTask(SKColors.CornflowerBlue),
            };

            var columnSeries2 = new ColumnSeries<float>
            {
                Values = values2,
                Stroke = null,
                Fill = new SolidColorPaintTask(SKColors.GreenYellow)
            };

            columnSeries1.PointMeasured += OnPointMeasured;
            columnSeries2.PointMeasured += OnPointMeasured;

            Series = new List<ISeries> { columnSeries1, columnSeries2 };
        }

        private void OnPointMeasured(IChartPoint<RectangleGeometry, LabelGeometry, SkiaSharpDrawingContext> point)
        {
            var visual = point.Context.Visual;
            var delayedFunction = new DelayedFunction(EasingFunctions.BuildCustomElasticOut(1.5f, 0.60f), point);

            visual
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
