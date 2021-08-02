using System.Windows.Controls;
using LiveChartsCore.SkiaSharpView.WPF;
using LiveChartsCore.SkiaSharpView.SKCharts;
using ViewModelsSamples.General.ChartToImage;
using System;

namespace WPFSample.General.ChartToImage
{
    /// <summary>
    /// Interaction logic for View.xaml
    /// </summary>
    public partial class View : UserControl
    {
        private readonly ViewModel _vm = new();

        public View()
        {
            InitializeComponent();

            _vm = new ViewModel();
            DataContext = _vm;

            Loaded += View_Loaded;
        }

        private void View_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // CARTESIAN CHART

            // you can create an image of a chart from memory using the
            // SKCartesianChart, SKPieChart or SKGeoMap classes.

            // in the case of this sample
            // the image was generated at the root folder ( samples/AvaloniaSample/bin/Debug/{targetFramework}/ )
            var cartesianChart = new SKCartesianChart
            {
                Width = 900,
                Height = 600,
                Series = _vm.CatesianSeries
            };

            // notice classes that implement ISkiaSharpChart (SKCartesianChart, SKPieChart and SKGeoMap classes)
            // do not require a UI you can use this objects installing only the
            // LiveChartsCore.SkiaSharpView package.

            // you can save the image to png (by default), or use the second argument to specify another format.
            cartesianChart.SaveImage("CartesianImageFromMemory.png"); // <- path where the image will be generated

            // alternatively you can get the image and do different operations:
            using var image = cartesianChart.GetImage();
            using var data = image.Encode();
            var base64 = Convert.ToBase64String(data.AsSpan());

            // or you could also use a chart in the user interface to create an image
            CreateImageFromCartesianControl();


            // PIE CHART
            new SKPieChart
            {
                Width = 900,
                Height = 600,
                Series = _vm.PieSeries
            }.SaveImage("PieImageFromMemory.png");

            // or create it from a control in the UI
            CreateImageFromPieControl();


            // GEO MAP CHART
            new SKGeoMap
            {
                Width = 900,
                Height = 600,
                Values = _vm.MapValues
            }.SaveImage("MapImageFromMemory.png");

            // or create it from a control in the UI
            CreateImageFromGeoControl();
        }

        private void CreateImageFromCartesianControl()
        {
            var chartControl = (CartesianChart)FindName("cartesianChart");
            var skChart = new SKCartesianChart(chartControl) { Width = 900, Height = 600, };
            skChart.SaveImage("CartesianImageFromControl.png");
        }

        private void CreateImageFromPieControl()
        {
            var chartControl = (PieChart)FindName("pieChart");
            var skChart = new SKPieChart(chartControl) { Width = 900, Height = 600, };
            skChart.SaveImage("PieImageFromControl.png");
        }

        private void CreateImageFromGeoControl()
        {
            var chartControl = (GeoMap)FindName("geoChart");
            var skChart = new SKGeoMap(chartControl) { Width = 900, Height = 600, };
            skChart.SaveImage("MapImageFromControl.png");
        }
    }
}
