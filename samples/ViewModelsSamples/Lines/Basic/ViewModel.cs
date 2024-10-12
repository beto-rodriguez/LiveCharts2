using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SkiaSharp;

namespace ViewModelsSamples.Lines.Basic;

public class ViewModel
{
    public ISeries[] Series { get; set; } = [
        new LineSeries<double>
        {
            Values = [2, 1, 3, 5, 3, 4, 6],
            Fill = null,
            GeometrySize = 20
        },
        new LineSeries<int, StarGeometry>
        {
            Values = [4, 2, 5, 2, 4, 5, 3],
            Fill = null,
            GeometrySize = 20,
            ScalesYAt = 1
        }
    ];

    public DrawMarginFrame Frame { get; set; } = new()
    {
        Stroke = new SolidColorPaint { Color = SKColors.Black, StrokeThickness = 1 },
    };

    public ICartesianAxis[] X { get; } = [
        new Axis {
            TicksPaint = new SolidColorPaint {Color = SKColors.Black },
            DrawTicksPath = true
        }
    ];

    public ICartesianAxis[] Y { get; } = [
        new Axis {
            TicksPaint = new SolidColorPaint {Color = SKColors.Green },
            DrawTicksPath = true
        },
        new Axis {
            Position = LiveChartsCore.Measure.AxisPosition.End,
            TicksPaint = new SolidColorPaint {Color = SKColors.Blue },
            DrawTicksPath = true
        }
    ];
}
