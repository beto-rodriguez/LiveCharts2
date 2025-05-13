using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using SkiaSharp;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;

namespace ViewModelsSamples.General.Sections;

public partial class ViewModel
{
    public ObservablePoint[] Values { get; set; } = [
        new ObservablePoint(2.2, 5.4),
        new ObservablePoint(4.5, 2.5),
        new ObservablePoint(4.2, 7.4),
        new ObservablePoint(6.4, 9.9),
        new ObservablePoint(8.9, 3.9),
        new ObservablePoint(9.9, 5.2)
    ];

    public RectangularSection[] Sections { get; set; } = [
        new RectangularSection
        {
            // creates a section from 3 to 4 in the X axis
            Xi = 3,
            Xj = 4,
            Fill = new SolidColorPaint(new SKColor(255, 205, 210))
        },

        new RectangularSection
        {
            // creates a section from 5 to 6 in the X axis
            // and from 2 to 8 in the Y axis
            Xi = 5,
            Xj = 6,
            Yi = 2,
            Yj = 8,
            Fill = new SolidColorPaint(new SKColor(187, 222, 251))
        },

        new RectangularSection
        {
            // creates a section from 8 to the end in the X axis
            Xi = 8,
            Fill = new SolidColorPaint(new SKColor(249, 251, 231)),
            Label = "A section here!",
            LabelSize = 14,
            LabelPaint = new SolidColorPaint(new SKColor(255, 111, 0))
        }
    ];

    [RelayCommand]
    public void ToggleFirst() =>
        Sections[0].IsVisible = !Sections[0].IsVisible;
}
