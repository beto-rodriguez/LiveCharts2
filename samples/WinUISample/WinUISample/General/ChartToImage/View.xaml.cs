using LiveChartsCore.SkiaSharpView.SKCharts;
using Microsoft.UI.Xaml.Controls;

namespace WinUISample.General.ChartToImage;

public sealed partial class View : UserControl
{
    public View()
    {
        InitializeComponent();

        // in this case when the view is loaded // mark
        // we render our chart controls as images // mark
        Loaded += View_Loaded;
    }

    private void View_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        CreateImageFromCartesianControl();
        CreateImageFromPieControl();
        CreateImageFromGeoControl();
    }

    private void CreateImageFromCartesianControl()
    {
        // you can take any chart in the UI, and build an image from it // mark
        var chartControl = cartesianChart;
        var skChart = new SKCartesianChart(chartControl) { Width = 900, Height = 600, };
        skChart.SaveImage("CartesianImageFromControl.png");
    }

    private void CreateImageFromPieControl()
    {
        var chartControl = pieChart;
        var skChart = new SKPieChart(chartControl) { Width = 900, Height = 600, };
        skChart.SaveImage("PieImageFromControl.png");
    }

    private void CreateImageFromGeoControl()
    {
        var chartControl = geoChart;
        var skChart = new SKGeoMap(chartControl) { Width = 900, Height = 600, };
        skChart.SaveImage("MapImageFromControl.png");
    }
}
