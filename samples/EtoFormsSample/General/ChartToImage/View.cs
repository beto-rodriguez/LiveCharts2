using Eto.Forms;
using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.SkiaSharpView.Eto.Forms;
using ViewModelsSamples.General.ChartToImage;

namespace EtoFormsSample.General.ChartToImage;

public class View : Panel
{
    private readonly CartesianChart _cartesian;
    private readonly PieChart _pie;
    private readonly GeoMap _map;

    public View()
    {
        InitializeComponent();
        Size = new Eto.Drawing.Size(90, 90);

        var viewModel = new ViewModel();

        // Adding a cartesian chart to the UI...
        _cartesian = new CartesianChart
        {
            Series = viewModel.CatesianSeries,

            // out of livecharts properties...
            Location = new Eto.Drawing.Point(0, 0),
            Size = new Eto.Drawing.Size(400, 200)
        };
        Controls.Add(_cartesian);

        // Adding a pie chart to the UI...
        _pie = new PieChart
        {
            Series = viewModel.PieSeries,

            // out of livecharts properties...
            Location = new Eto.Drawing.Point(0, 200),
            Size = new Eto.Drawing.Size(400, 200)
        };
        Controls.Add(_pie);

        // Adding a map chart to the UI...
        _map = new GeoMap
        {
            Shapes = viewModel.MapShapes,

            // out of livecharts properties...
            Location = new Eto.Drawing.Point(0, 400),
            Size = new Eto.Drawing.Size(400, 200)
        };
        Controls.Add(_map);

        // now lets create the images // mark
        CreateImageFromCartesianControl(); // mark
        CreateImageFromPieControl(); // mark
        CreateImageFromGeoControl(); // mark
    }

    private void CreateImageFromCartesianControl()
    {
        // you can take any chart in the UI, and build an image from it // mark
        var chartControl = _cartesian;
        var skChart = new SKCartesianChart(chartControl) { Width = 900, Height = 600, };
        skChart.SaveImage("CartesianImageFromControl.png");
    }

    private void CreateImageFromPieControl()
    {
        var chartControl = _pie;
        var skChart = new SKPieChart(chartControl) { Width = 900, Height = 600, };
        skChart.SaveImage("PieImageFromControl.png");
    }

    private void CreateImageFromGeoControl()
    {
        var chartControl = _map;
        var skChart = new SKGeoMap(chartControl) { Width = 900, Height = 600, };
        skChart.SaveImage("MapImageFromControl.png");
    }
}
