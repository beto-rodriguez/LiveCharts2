using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Xamarin.Forms;
using ViewModelsSamples.Events.AddPointOnClick;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinSample.Events.AddPointOnClick;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class View : ContentPage
{
    public View()
    {
        InitializeComponent();
    }

    private void Chart_Touched(object sender, SkiaSharp.Views.Forms.SKTouchEventArgs e)
    {
        var chart = (CartesianChart)FindByName("chart");

        var viewModel = (ViewModel)BindingContext;

        // scales the UI coordinates to the corresponding data in the chart.
        var dataCoordinates = chart.ScalePixelsToData(new LvcPointD(e.Location.X, e.Location.Y));

        // finally add the new point to the data in our chart.
        viewModel?.Data.Add(new ObservablePoint(dataCoordinates.X, dataCoordinates.Y));

        // You can also get all the points or visual elements in a given location.
        var points = chart.GetPointsAt(new LvcPoint((float)e.Location.X, (float)e.Location.Y));
        var visuals = chart.GetVisualsAt(new LvcPoint((float)e.Location.X, (float)e.Location.Y));
    }
}
