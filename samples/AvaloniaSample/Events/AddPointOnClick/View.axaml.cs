using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Avalonia;
using ViewModelsSamples.Events.AddPointOnClick;

namespace AvaloniaSample.Events.AddPointOnClick;

public class View : UserControl
{
    public View()
    {
        InitializeComponent();
    }

    private void ChartPointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        var chart = this.FindControl<CartesianChart>("chart");
        var viewModel = DataContext as ViewModel;

        // gets the point in the UI coordinates.
        var p = e.GetPosition(chart);

        // scales the UI coordinates to the corresponding data in the chart.
        var dataCoordinates = chart.ScalePixelsToData(new LvcPointD(p.X, p.Y));

        // finally add the new point to the data in our chart.
        viewModel?.Data.Add(new ObservablePoint(dataCoordinates.X, dataCoordinates.Y));

        // You can also get all the points or visual elements in a given location.
        var points = chart.GetPointsAt(new LvcPoint(p.X, p.Y));
        var visuals = chart.GetVisualsAt(new LvcPoint(p.X, p.Y));
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
