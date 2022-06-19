using System.Windows.Controls;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.WPF;
using ViewModelsSamples.Events.AddPointOnClick;

namespace WPFSample.Events.AddPointOnClick;

/// <summary>
/// Interaction logic for View.xaml
/// </summary>
public partial class View : UserControl
{
    public View()
    {
        InitializeComponent();
    }

    private void Chart_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        var chart = (CartesianChart)FindName("chart");
        var viewModel = (ViewModel)DataContext;

        // gets the point in the UI coordinates.
        var p = e.GetPosition(chart);

        // scales the UI coordinates to the corresponding data in the chart.
        // ScaleUIPoint returns an array of double
        var scaledPoint = chart.ScaleUIPoint(new LvcPoint((float)p.X, (float)p.Y));

        // where the X coordinate is in the first position
        var x = scaledPoint[0];

        // and the Y coordinate in the second position
        var y = scaledPoint[1];

        // finally add the new point to the data in our chart.
        viewModel.Data.Add(new ObservablePoint(x, y));
    }
}
