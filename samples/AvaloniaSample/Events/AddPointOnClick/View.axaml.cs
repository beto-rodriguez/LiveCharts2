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

        // scales the UI coordintaes to the corresponging data in the chart.
        // ScaleUIPoint retuns an array of double
        var scaledPoint = chart.ScaleUIPoint(new LvcPoint((float)p.X, (float)p.Y));

        // where the X coordinate is in the first position
        var x = scaledPoint[0];

        // and the Y coordinate in the second position
        var y = scaledPoint[1];

        // finally add the new point to the data in our chart.
        viewModel?.Data.Add(new ObservablePoint(x, y));
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
