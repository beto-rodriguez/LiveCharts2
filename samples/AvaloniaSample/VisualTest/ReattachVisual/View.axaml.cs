using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LiveChartsCore.SkiaSharpView.Avalonia;

namespace AvaloniaSample.VisualTest.ReattachVisual;

public partial class View : UserControl
{
    private bool _isInVisualTree = true;

    public View()
    {
        InitializeComponent();
    }

    private void OnToggleAttach(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var parent = this.FindControl<Grid>("parent");
        var chart = this.FindControl<CartesianChart>("chart");
        var pieChart = this.FindControl<PieChart>("pieChart");
        var polarChart = this.FindControl<PolarChart>("polarChart");

        if (_isInVisualTree)
        {
            _ = parent.Children.Remove(chart);
            _ = parent.Children.Remove(pieChart);
            _ = parent.Children.Remove(polarChart);
            _isInVisualTree = false;
            return;
        }

        parent.Children.Add(chart);
        parent.Children.Add(pieChart);
        parent.Children.Add(polarChart);
        _isInVisualTree = true;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
