using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.SkiaSharpView.UWP;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPSample.General.ChartToImage
{
    public sealed partial class View : UserControl
    {
        public View()
        {
            InitializeComponent();
            Loaded += View_Loaded;
        }

        private void View_Loaded(object sender, RoutedEventArgs e)
        {
            CreateImageFromCartesianControl();
            CreateImageFromPieControl();
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
