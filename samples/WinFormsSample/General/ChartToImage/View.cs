using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.SkiaSharpView.WinForms;
using ViewModelsSamples.General.ChartToImage;

namespace WinFormsSample.General.ChartToImage;

public partial class View : UserControl
{
    private readonly CartesianChart _cartesian;
    private readonly PieChart _pie;
    private readonly GeoMap _map;

    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(420, 620);

        var viewModel = new ViewModel();

        // Adding a cartesian chart to the UI...
        _cartesian = new CartesianChart
        {
            Series =
            [
                new LiveChartsCore.SkiaSharpView.ColumnSeries<double> { Values = viewModel.Values1 },
                new LiveChartsCore.SkiaSharpView.LineSeries<double> { Values = viewModel.Values2 }
            ],
            Location = new System.Drawing.Point(10, 10),
            Size = new System.Drawing.Size(400, 180)
        };
        Controls.Add(_cartesian);

        // Adding a pie chart to the UI...
        _pie = new PieChart
        {
            Series =
            [
                new LiveChartsCore.SkiaSharpView.PieSeries<double> { Values = viewModel.PieValues1 },
                new LiveChartsCore.SkiaSharpView.PieSeries<double> { Values = viewModel.PieValues2 },
                new LiveChartsCore.SkiaSharpView.PieSeries<double> { Values = viewModel.PieValues3 }
            ],
            Location = new System.Drawing.Point(10, 210),
            Size = new System.Drawing.Size(400, 180)
        };
        Controls.Add(_pie);

        // Adding a map chart to the UI...
        _map = new GeoMap
        {
            // For demonstration, leave Series empty or add a sample series if available
            Location = new System.Drawing.Point(10, 410),
            Size = new System.Drawing.Size(400, 180)
        };
        Controls.Add(_map);

        // now lets create the images
        CreateImageFromCartesianControl();
        CreateImageFromPieControl();
        CreateImageFromGeoControl();
    }

    private void CreateImageFromCartesianControl()
    {
        var chartControl = _cartesian;
        var skChart = new SKCartesianChart(chartControl) { Width = 900, Height = 600 };
        skChart.SaveImage("CartesianImageFromControl.png");
    }

    private void CreateImageFromPieControl()
    {
        var chartControl = _pie;
        var skChart = new SKPieChart(chartControl) { Width = 900, Height = 600 };
        skChart.SaveImage("PieImageFromControl.png");
    }

    private void CreateImageFromGeoControl()
    {
        var chartControl = _map;
        var skChart = new SKGeoMap(chartControl) { Width = 900, Height = 600 };
        skChart.SaveImage("MapImageFromControl.png");
    }
}
