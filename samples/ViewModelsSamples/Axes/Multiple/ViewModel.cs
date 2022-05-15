using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Axes.Multiple;

[ObservableObject]
public partial class ViewModel
{
    private static readonly SKColor s_blue = new(25, 118, 210);
    private static readonly SKColor s_red = new(229, 57, 53);
    private static readonly SKColor s_yellow = new(198, 167, 0);

    public ISeries[] Series { get; set; } =
    {
        new LineSeries<double>
        {
            LineSmoothness = 1,
            Name = "Tens",
            Values = new double[] { 14, 13, 14, 15, 17 },
            Stroke = new SolidColorPaint(s_blue, 2),
            GeometrySize = 10,
            GeometryStroke = new SolidColorPaint(s_blue, 2),
            Fill = null,
            ScalesYAt = 0 // it will be scaled at the Axis[0] instance // mark
        },
        new LineSeries<double>
        {
            Name = "Tens 2",
            Values = new double[] { 11, 12, 13, 10, 13 },
            Stroke = new SolidColorPaint(s_blue, 2),
            GeometrySize = 10,
            GeometryStroke = new SolidColorPaint(s_blue, 2),
            Fill = null,
            ScalesYAt = 0 // it will be scaled at the Axis[0] instance // mark
        },
        new LineSeries<double>
        {
            Name = "Hundreds",
            Values = new double[] { 533, 586, 425, 579, 518 },
            Stroke = new SolidColorPaint(s_red, 2),
            GeometrySize = 10,
            GeometryStroke = new SolidColorPaint(s_red, 2),
            Fill = null,
            ScalesYAt = 1 // it will be scaled at the YAxes[1] instance // mark
        },
        new LineSeries<double>
        {
            Name = "Thousands",
            Values = new double[] { 5493, 7843, 4368, 9018, 3902 },
            Stroke = new SolidColorPaint(s_yellow, 2),
            GeometrySize = 10,
            GeometryStroke = new SolidColorPaint(s_yellow, 2),
            Fill = null,
            ScalesYAt = 2  // it will be scaled at the YAxes[2] instance // mark
        }
    };

    public ICartesianAxis[] YAxes { get; set; } =
    {
        new Axis // the "units" and "tens" series will be scaled on this axis
        {
            Name = "Tens",
            NameTextSize = 14,
            NamePaint = new SolidColorPaint(s_blue),
            TextSize = 12,
            LabelsPaint = new SolidColorPaint(s_blue),
        },
        new Axis // the "hundreds" series will be scaled on this axis
        {
            Name = "Hundreds",
            NameTextSize = 14,
            NamePaint = new SolidColorPaint(s_red),
            TextSize = 12,
            LabelsPaint = new SolidColorPaint(s_red),
            ShowSeparatorLines = false,
            Position = LiveChartsCore.Measure.AxisPosition.End
        },
        new Axis // the "thousands" series will be scaled on this axis
        {
            Name = "Thousands",
            NameTextSize = 14,
            NamePaint = new SolidColorPaint(s_yellow),
            TextSize = 12,
            LabelsPaint = new SolidColorPaint(s_yellow),
            ShowSeparatorLines = false,
            Position = LiveChartsCore.Measure.AxisPosition.End
        }
    };
}
