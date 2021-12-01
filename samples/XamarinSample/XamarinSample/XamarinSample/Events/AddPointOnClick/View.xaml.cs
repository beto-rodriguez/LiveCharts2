using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ViewModelsSamples.Events.AddPointOnClick;
using LiveChartsCore.SkiaSharpView.Xamarin.Forms;

namespace XamarinSample.Events.AddPointOnClick
{
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

            // scales the UI coordintaes to the corresponging data in the chart.
            // ScaleUIPoint retuns an array of double
            var scaledPoint = chart.ScaleUIPoint(new LvcPoint(e.Location.X, e.Location.Y));

            // where the X coordinate is in the first position
            var x = scaledPoint[0];

            // and the Y coordinate in the second position
            var y = scaledPoint[1];

            // finally add the new point to the data in our chart.
            viewModel.Data.Add(new ObservablePoint(x, y));
        }
    }
}
