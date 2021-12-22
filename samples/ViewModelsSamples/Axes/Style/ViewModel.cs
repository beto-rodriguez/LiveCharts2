using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using SkiaSharp;

namespace ViewModelsSamples.Axes.Style;

public class ViewModel
{
    public IEnumerable<ISeries> Series { get; set; }
        = new ISeries[] {
            new LineSeries<int>
            {
                Values = new int[] { 5, 3, 2, 8, 7, 4, 5, 4, 6, 7, 5 },
                Fill = null,
                GeometrySize = 0
            }
        };

    public IEnumerable<Axis> XAxes { get; set; }
        = new Axis[]
        {
            new Axis
            {
                MinLimit = 0,
                MaxLimit = 10,
                ForceStepToMin = true,
                MinStep = 0.5,
                TextSize = 14,
                SeparatorsPaint = new SolidColorPaint
                {
                    Color = SKColors.Gray,
                    StrokeThickness = 2
                }
            }
        };

    public IEnumerable<ICartesianAxis> YAxes { get; set; }
        = new Axis[]
        {
            new Axis
            {
                MinLimit = 0,
                MaxLimit = 10,
                ForceStepToMin = true,
                MinStep = 1,
                TextSize = 14,
                SeparatorsPaint = new SolidColorPaint
                {
                    Color = SKColors.Gray,
                    StrokeThickness = 2,
                    PathEffect = new DashEffect(new float[] { 3, 3 })
                }
            }
        };
}
