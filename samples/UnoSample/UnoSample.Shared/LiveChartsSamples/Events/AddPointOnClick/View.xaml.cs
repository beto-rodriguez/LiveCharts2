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
        // ScaleUIPoint returns an array of double
        var scaledPoint = chart.ScaleUIPoint(new LvcPoint((float)p.Position.X, (float)p.Position.Y));

        // where the X coordinate is in the first position
        var x = scaledPoint[0];

        // and the Y coordinate in the second position
        var y = scaledPoint[1];

        // finally add the new point to the data in our chart.
        viewModel.Data.Add(new ObservablePoint(x, y));
    }
}
