using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using LiveChartsCore.SkiaSharpView.SKCharts;
using ViewModelsSamples.General.ChartToImage;
using LiveChartsCore.SkiaSharpView.Xamarin.Forms;
using System.IO;
using System;

namespace XamarinSample.General.ChartToImage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class View : ContentPage
    {
        public View()
        {
            InitializeComponent();

            var vm = new ViewModel();
            BindingContext = vm;

            // CARTESIAN CHART

            // you can create a image of a chart from memory using the
            // SKCartesianChart, SKPieChart or SKGeoMap controls.

            // in the case of this sample
            new SKCartesianChart
            {
                Width = 900,
                Height = 600,
                Series = vm.CaterianSeries
            }.SaveImage(
                // path where the image will be generated
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CartesianImageFromMemory.png"));

            // or you could also use a chart in the user interface to create an image
            CreateImageFromCartesianControl();


            // PIE CHART
            new SKPieChart
            {
                Width = 900,
                Height = 600,
                Series = vm.PieSeries
            }.SaveImage(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PieImageFromMemory.png"));

            // or create it from a control in the UI
            CreateImageFromPieControl();


            // GEO MAP CHART
            new SKGeoMap
            {
                Width = 900,
                Height = 600,
                Values = vm.MapValues
            }.SaveImage(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MapImageFromMemory.png"));

            // or create it from a control in the UI
            CreateImageFromGeoControl();
        }

        private void CreateImageFromCartesianControl()
        {
            var chartControl =  (CartesianChart)FindByName("cartesianChart");
            var skChart = new SKCartesianChart(chartControl) { Width = 900, Height = 600, };
            skChart.SaveImage(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CartesianImageFromControl.png"));
        }

        private void CreateImageFromPieControl()
        {
            var chartControl = (PieChart)FindByName("pieChart");
            var skChart = new SKPieChart(chartControl) { Width = 900, Height = 600, };
            skChart.SaveImage(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PieImageFromControl.png"));
        }

        private void CreateImageFromGeoControl()
        {
            var chartControl = (GeoMap)FindByName("geoChart");
            var skChart = new SKGeoMap(chartControl) { Width = 900, Height = 600, };
            skChart.SaveImage(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MapImageFromControl.png"));
        }
    }
}
