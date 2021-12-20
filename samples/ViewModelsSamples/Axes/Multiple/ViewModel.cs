using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Axes.Multiple;

public class ViewModel
{
    public ViewModel()
    {
        var blue = new SKColor(25, 118, 210);
        var red = new SKColor(229, 57, 53);
        var yellow = new SKColor(198, 167, 0);

        Series = new ISeries[]
        {
            new LineSeries<double>
            {
                LineSmoothness = 1,
                Name = "Tens",
                Values = new ObservableCollection<double> { 14, 13, 14, 15, 17 },
                Stroke = new SolidColorPaint(blue, 2),
                GeometrySize = 10,
                GeometryStroke = new SolidColorPaint(blue, 2),
                Fill = null,
                ScalesYAt = 0 // it will be scaled at the Axis[0] instance // mark
            },
            new LineSeries<double>
            {
                Name = "Tens 2",
                Values = new ObservableCollection<double> { 11, 12, 13, 10, 13 },
                Stroke = new SolidColorPaint(blue, 2),
                GeometrySize = 10,
                GeometryStroke = new SolidColorPaint(blue, 2),
                Fill = null,
                ScalesYAt = 0 // it will be scaled at the Axis[0] instance // mark
            },
            new LineSeries<double>
            {
                Name = "Hundreds",
                Values = new ObservableCollection<double> { 533, 586, 425, 579, 518 },
                Stroke = new SolidColorPaint(red, 2),
                GeometrySize = 10,
                GeometryStroke = new SolidColorPaint(red, 2),
                Fill = null,
                ScalesYAt = 1 // it will be scaled at the YAxes[1] instance // mark
            },
            new LineSeries<double>
            {
                Name = "Thousands",
                Values = new ObservableCollection<double> { 5493, 7843, 4368, 9018, 3902 },
                Stroke = new SolidColorPaint(yellow, 2),
                GeometrySize = 10,
                GeometryStroke = new SolidColorPaint(yellow, 2),
                Fill = null,
                ScalesYAt = 2  // it will be scaled at the YAxes[2] instance // mark
            }
        };

        YAxes = new Axis[]
        {
            new Axis // the "units" and "tens" series will be scaled on this axis
            {
                Name = "Tens",
                NameTextSize = 14,
                NamePaint = new SolidColorPaint(blue),
                TextSize = 12,
                LabelsPaint = new SolidColorPaint(blue),
            },
            new Axis // the "hundreds" series will be scaled on this axis
            {
                Name = "Hundreds",
                NameTextSize = 14,
                NamePaint = new SolidColorPaint(red),
                TextSize = 12,
                LabelsPaint = new SolidColorPaint(red),
                ShowSeparatorLines = false,
                Position = LiveChartsCore.Measure.AxisPosition.End
            },
            new Axis // the "thousands" series will be scaled on this axis
            {
                Name = "Thousands",
                NameTextSize = 14,
                NamePaint = new SolidColorPaint(yellow),
                TextSize = 12,
                LabelsPaint = new SolidColorPaint(yellow),
                ShowSeparatorLines = false,
                Position = LiveChartsCore.Measure.AxisPosition.End
            }
        };
    }

    public ISeries[] Series { get; set; }
    public ICartesianAxis[] YAxes { get; set; }
}
