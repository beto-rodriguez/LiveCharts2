using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.SkiaSharpView.WinUI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;

namespace UnoWinUISample.General.ChartToImage;

public sealed partial class View : UserControl
{
    public View()
    {
        InitializeComponent();

        // in this case, when the view is loaded // mark
        // we render our chart controls as images // mark
        Loaded += View_Loaded;
    }

    private void View_Loaded(object? sender, RoutedEventArgs e)
    {
        CreateImageFromCartesianControl();
        CreateImageFromPieControl();
        CreateImageFromGeoControl();
    }

    private void CreateImageFromCartesianControl()
    {
        // you can take any chart in the UI, and build an image from it // mark
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
