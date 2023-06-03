using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using ViewModelsSamples.Events.AddPointOnClick;
using Windows.UI.Xaml.Controls;

namespace UnoSample.Events.AddPointOnClick;

public sealed partial class View : UserControl
{
    public View()
    {
        InitializeComponent();
    }

    private void Chart_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var viewModel = (ViewModel)DataContext;

        // gets the point in the UI coordinates.
        var p = e.GetCurrentPoint(chart);

        // scales the UI coordinates to the corresponding data in the chart.
        var dataCoordinates = chart.ScalePixelsToData(new LvcPointD(p.Position.X, p.Position.Y));

        // finally add the new point to the data in our chart.
        viewModel?.Data.Add(new ObservablePoint(dataCoordinates.X, dataCoordinates.Y));

        // You can also get all the points or visual elements in a given location.
        var points = chart.GetPointsAt(new LvcPoint((float)p.Position.X, (float)p.Position.Y));
        var visuals = chart.GetVisualsAt(new LvcPoint((float)p.Position.X, (float)p.Position.Y));
    }
}
