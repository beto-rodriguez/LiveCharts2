using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Themes;
using SkiaSharp;

namespace ViewModelsSamples.Axes.ColorsAndPosition;

public partial class ViewModel
{
    private AxisPosition _selectedPosition = AxisPosition.End;
    private int _selectedColor = 0;
    private readonly LvcColor[] _colors = ColorPalletes.FluentDesign;

    public ISeries[] Series { get; set; } = [
        new ColumnSeries<double>
        {
            Values = new ObservableCollection<double> { 2, 5, 4, -2, 4, -3, 5 },
            Stroke = null,
            Fill = new SolidColorPaint { Color = SKColors.DarkOliveGreen }
        }
    ];

    public Axis[] XAxes { get; set; } = [
        new Axis
        {
            //Name = "X axis",
            TextSize = 20,

            // LabelsPaint = null will not draw the axis labels.
            LabelsPaint = new SolidColorPaint{ Color = SKColors.CornflowerBlue },

            // SeparatorsPaint = null will not draw the separator lines
            SeparatorsPaint = new SolidColorPaint { Color = SKColors.LightBlue, StrokeThickness = 3 },

            Position = AxisPosition.End
        }
    ];

    public Axis[] YAxes { get; set; } = [
        new Axis
        {
            //Name = "Y axis",
            TextSize = 20,
            LabelsPaint = new SolidColorPaint { Color = SKColors.Red },
            SeparatorsPaint = new SolidColorPaint { Color = SKColors.LightPink, StrokeThickness = 3 },
            Position = AxisPosition.End
        }
    ];

    [RelayCommand]
    public void SetNewColor()
    {
        var nextColor = _colors[_selectedColor++ % _colors.Length];
        XAxes[0].LabelsPaint = new SolidColorPaint(new SKColor(nextColor.R, nextColor.G, nextColor.B));
        XAxes[0].SeparatorsPaint = new SolidColorPaint(new SKColor(nextColor.R, nextColor.G, nextColor.B), 3);
    }

    [RelayCommand]
    public void TogglePosition()
    {
        _selectedPosition = _selectedPosition == AxisPosition.End ? AxisPosition.Start : AxisPosition.End;
        XAxes[0].Position = _selectedPosition;
        YAxes[0].Position = _selectedPosition;
    }
}
