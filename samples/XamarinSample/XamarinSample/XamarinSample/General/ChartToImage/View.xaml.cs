using System;
using System.IO;
using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.SkiaSharpView.Xamarin.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinSample.General.ChartToImage;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class View : ContentPage
{
    private readonly string _folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

    public View()
    {
        InitializeComponent();

        CreateImageFromCartesianControl();
        CreateImageFromPieControl();
        CreateImageFromGeoControl();
    }

    private void CreateImageFromCartesianControl()
    {
        var chartControl = (CartesianChart)FindByName("cartesianChart");
        var skChart = new SKCartesianChart(chartControl) { Width = 900, Height = 600, };
        skChart.SaveImage(Path.Combine(_folderPath, "CartesianImageFromControl.png"));
    }

    private void CreateImageFromPieControl()
    {
        var chartControl = (PieChart)FindByName("pieChart");
        var skChart = new SKPieChart(chartControl) { Width = 900, Height = 600, };
        skChart.SaveImage(Path.Combine(_folderPath, "PieImageFromControl.png"));
    }

    private void CreateImageFromGeoControl()
    {
        var chartControl = (GeoMap)FindByName("geoChart");
        var skChart = new SKGeoMap(chartControl) { Width = 900, Height = 600, };
        skChart.SaveImage(Path.Combine(_folderPath, "MapImageFromControl.png"));
    }
}
