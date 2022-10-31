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
        var scaledPoint = chart.ScalePixelsToData(new LvcPointD(p.X, p.Y));

        // finally add the new point to the data in our chart.
        viewModel.Data.Add(new ObservablePoint(scaledPoint.X, scaledPoint.Y));
    }
}
