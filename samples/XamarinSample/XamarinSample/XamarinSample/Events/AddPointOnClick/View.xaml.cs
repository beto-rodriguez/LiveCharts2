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
        // ScaleUIPoint returns an array of double
        var scaledPoint = chart.ScaleUIPoint(new LvcPoint(e.Location.X, e.Location.Y));

        // where the X coordinate is in the first position
        var x = scaledPoint[0];

        // and the Y coordinate in the second position
        var y = scaledPoint[1];

        // finally add the new point to the data in our chart.
        viewModel.Data.Add(new ObservablePoint(x, y));
    }
}
