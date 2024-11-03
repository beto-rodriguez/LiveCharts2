using System.Collections.Generic;
using System.Linq;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Drawing;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace ViewModelsSamples.Events.OverrideFind;

public class ViewModel
{
    public ISeries[] Series { get; set; } = [
        new CustomColumnSeries<int> { Values = [9, 5, 7, 3, 7, 3] },
        new CustomColumnSeries<int> { Values = [8, 2, 3, 2, 5, 2] }
    ];

    public class CustomColumnSeries<T> : ColumnSeries<T>
    {
        protected override IEnumerable<ChartPoint> FindPointsInPosition(
            IChart chart, LvcPoint pointerPosition, TooltipFindingStrategy strategy, FindPointFor findPointFor)
        {
            // use the default implementation for pointer down events.
            // if (findPointFor == FindPointFor.PointerDownEvent)
            //     return base.FindPointsInPosition(chart, pointerPosition, strategy, findPointFor);

            return Fetch(chart).Where(point =>
            {
                var animatable = (Animatable)point.Context.Visual!;

                var x = GetTargetValue(animatable, nameof(RoundedRectangleGeometry.X));
                var y = GetTargetValue(animatable, nameof(RoundedRectangleGeometry.Y));
                var w = GetTargetValue(animatable, nameof(RoundedRectangleGeometry.Width));
                var h = GetTargetValue(animatable, nameof(RoundedRectangleGeometry.Height));

                var hoverArea = (RectangleHoverArea)point.Context.HoverArea!;
                hoverArea.SuggestedTooltipLocation = new LvcPoint(x + w * 0.5f, y);

                var isInsideX = x <= pointerPosition.X && pointerPosition.X <= x + w;
                var isInsideY = y <= pointerPosition.Y && pointerPosition.Y <= y + h;

                return isInsideX && isInsideY;
            });
        }

        private static float GetTargetValue(Animatable animatable, string name)
        {
            // get the motion property in the shape that handles animations.
            var motionProperty = (MotionProperty<float>)animatable.MotionProperties[name];

            // return the target value, it is the value where the animation will end.
            return motionProperty.ToValue;
        }
    }
}
