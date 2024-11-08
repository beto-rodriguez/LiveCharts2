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
            IChart chart, LvcPoint pointerPosition, FindingStrategy strategy, FindPointFor findPointFor)
        {
            // Overriding the FindPointsInPosition method allows us to customize the way the library
            // finds the points in the chart for a given position.
            // this is used for tooltips, pointer down events, or the Chart.GetPointsAt() method.

            // you can use the findPointFor parameter to determine the context of the search.

            // if (findPointFor == FindPointFor.PointerDownEvent)
            //     return ...

            // in this case we want only the points that are exactly under the pointer position.

            return Fetch(chart).Where(point =>
            {
                var animatable = (Animatable)point.Context.Visual!;

                // we use the GetTargetValue to get the target value of the animation
                var x = GetTargetValue(animatable, nameof(RoundedRectangleGeometry.X));
                var y = GetTargetValue(animatable, nameof(RoundedRectangleGeometry.Y));
                var w = GetTargetValue(animatable, nameof(RoundedRectangleGeometry.Width));
                var h = GetTargetValue(animatable, nameof(RoundedRectangleGeometry.Height));

                // hover areas, are used to determine the zone in the chart where the point
                // is considered hovered, in this case we are updating the suggested tooltip location,
                // to place it at the top of the column.
                var hoverArea = (RectangleHoverArea)point.Context.HoverArea!;
                hoverArea.SuggestedTooltipLocation = new LvcPoint(x + w * 0.5f, y);

                // finally we return true when the pointerPosition is contained
                // in the rectangle of the drawn column.

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
