using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LiveChartsCore.SkiaSharpView.Avalonia;
using LiveChartsCore.SkiaSharpView.SKCharts;

namespace AvaloniaSample.General.ChartToImage;

public partial class View : UserControl
{
    public View()
    {
        InitializeComponent();

        // in this case in the constructor of this view // mark
        // we render our chart controls as images // mark
        CreateImageFromCartesianControl();
        CreateImageFromPieControl();
        CreateImageFromGeoControl();
    }

    private void CreateImageFromCartesianControl()
    {
        // you can take any chart in the UI, and build an image from it // mark
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
