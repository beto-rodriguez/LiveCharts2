using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LiveChartsCore.SkiaSharpView.Avalonia;
using LiveChartsCore.SkiaSharpView.SKCharts;
using ViewModelsSamples.General.ChartToImage;

namespace AvaloniaSample.General.ChartToImage
{
    public class View : UserControl
    {
        public View()
        {
            InitializeComponent();

            var vm = new ViewModel();
            DataContext = vm;

            // CARTESIAN CHART

            // you can create a image of a chart from memory using the
            // SKCartesianChart, SKPieChart or SKGeoMap controls.

            // in the case of this sample
            // the image was generated at the root folder ( samples/AvaloniaSample/bin/Debug/{targetFramework}/ )
            new SKCartesianChart
            {
                Width = 900,
                Height = 600,
                Series = vm.CaterianSeries
            }.SaveImage("CartesianImageFromMemory.png"); // <- path where the image will be generated

            // or you could also use a chart in the user interface to create an image
            CreateImageFromCartesianControl();


            // PIE CHART
            new SKPieChart
            {
                Width = 900,
                Height = 600,
                Series = vm.PieSeries
            }.SaveImage("PieImageFromMemory.png");

            // or create it from a control in the UI
            CreateImageFromPieControl();


            // GEO MAP CHART
            new SKGeoMap
            {
                Width = 900,
                Height = 600,
                Values = vm.MapValues
            }.SaveImage("MapImageFromMemory.png");

            // or create it from a control in the UI
            CreateImageFromGeoControl();
        }

        private void CreateImageFromCartesianControl()
        {
            var chartControl = this.FindControl<CartesianChart>("cartesianChart");
            var skChart = new SKCartesianChart(chartControl) { Width = 900, Height = 600, };
            skChart.SaveImage("CartesianImageFromControl.png");
        }

        private void CreateImageFromPieControl()
        {
            var chartControl = this.FindControl<PieChart>("pieChart");
            var skChart = new SKPieChart(chartControl) { Width = 900, Height = 600, };
            skChart.SaveImage("PieImageFromControl.png");
        }

        private void CreateImageFromGeoControl()
        {
            var chartControl = this.FindControl<GeoMap>("geoChart");
            var skChart = new SKGeoMap(chartControl) { Width = 900, Height = 600, };
            skChart.SaveImage("MapImageFromControl.png");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
