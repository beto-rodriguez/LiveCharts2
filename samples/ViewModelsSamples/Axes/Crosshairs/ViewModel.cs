using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Axes.Crosshairs;

public partial class ViewModel : ObservableObject
{
    public ISeries[] Series { get; set; } =
    {
        new LineSeries<double>
        {
            Values = new ObservableCollection<double> { 200, 558, 458, 249, 457, 339, 587 },
        },
        new LineSeries<double>
        {
            Values = new ObservableCollection<double> { 210, 400, 300, 350, 219, 323, 618 },
        },
    };

    public Axis[] XAxes { get; set; } =
    {
        new Axis
        {
            CrosshairLabelsBackground = SKColors.DarkOrange.AsLvcColor(),
            CrosshairLabelsPaint = new SolidColorPaint(SKColors.DarkRed, 1),
            CrosshairPaint = new SolidColorPaint(SKColors.DarkOrange, 1),
            Labeler = value => value.ToString("N2")
        }
    };
    public Axis[] YAxes { get; set; } =
    {
        new Axis
        {
            CrosshairLabelsBackground = SKColors.DarkOrange.AsLvcColor(),
            CrosshairLabelsPaint = new SolidColorPaint(SKColors.DarkRed, 1),
            CrosshairPaint = new SolidColorPaint(SKColors.DarkOrange, 1),
            CrosshairSnapEnabled = true // snapping is also supported
        }
    };
}
