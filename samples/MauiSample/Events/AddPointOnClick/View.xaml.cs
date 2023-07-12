using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using ViewModelsSamples.Events.AddPointOnClick;

namespace MauiSample.Events.AddPointOnClick;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class View : ContentPage
{
    public View()
    {
        InitializeComponent();
    }

    private void OnTapped(object sender, TappedEventArgs e)
    {
        var viewModel = (ViewModel)BindingContext;
        var position = e.GetPosition(chart).Value;
        var p = new LvcPointD(position.X, position.Y);

        // scales the UI coordinates to the corresponding data in the chart.
        var dataCoordinates = chart.ScalePixelsToData(p);

        // finally add the new point to the data in our chart.
        viewModel?.Data.Add(new ObservablePoint(dataCoordinates.X, dataCoordinates.Y));

        // You can also get all the points or visual elements in a given location.
        var points = chart.GetPointsAt(new LvcPoint(p.X, p.Y));
        var visuals = chart.GetVisualsAt(new LvcPoint(p.X, p.Y));
    }
}
