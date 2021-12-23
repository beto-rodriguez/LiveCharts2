using System.Windows.Controls;
using LiveChartsCore.SkiaSharpView.WPF;

namespace WPFSample.VisualTest.ReattachVisual;

/// <summary>
/// Interaction logic for View.xaml
/// </summary>
public partial class View : UserControl
{
    private bool _isInVisualTree = true;

    public View()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        var parent = (Grid)FindName("parent");
        var chart = (CartesianChart)FindName("chart");

        if (_isInVisualTree)
        {
            parent.Children.Remove(chart);
            _isInVisualTree = false;
            return;
        }

        _ = parent.Children.Add(chart);
        _isInVisualTree = true;
    }
}
