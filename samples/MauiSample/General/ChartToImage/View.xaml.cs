using System;
using System.IO;
using LiveChartsCore.SkiaSharpView.Maui;
using LiveChartsCore.SkiaSharpView.SKCharts;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using ViewModelsSamples.General.ChartToImage;

namespace MauiSample.General.ChartToImage
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

            // you can create an image of a chart from memory using the
            // SKCartesianChart, SKPieChart or SKGeoMap classes.

            // in the case of this sample
            var cartesianChart = new SKCartesianChart
            {
                Width = 900,
                Height = 600,
                Series = vm.CatesianSeries
            };

            // notice classes that implement ISkiaSharpChart (SKCartesianChart, SKPieChart and SKGeoMap classes)
            // do not require a UI you can use this objects installing only the
            // LiveChartsCore.SkiaSharpView package.

            // you can save the image to png (by default), or use the second argument to specify another format.
            cartesianChart.SaveImage("CartesianImageFromMemory.png"); // <- path where the image will be generated

            // alternatively you can get the image and do different operations:
            using var image = cartesianChart.GetImage();
            using var data = image.Encode();
            var base64 = Convert.ToBase64String(data.AsSpan().ToArray());

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
                Shapes = vm.MapShapes
            }.SaveImage(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MapImageFromMemory.png"));

            // or create it from a control in the UI
            CreateImageFromGeoControl();
        }

        private void CreateImageFromCartesianControl()
        {
            var chartControl = (CartesianChart)FindByName("cartesianChart");
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
