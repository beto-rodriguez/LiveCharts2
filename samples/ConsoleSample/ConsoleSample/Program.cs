using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Geo;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SkiaSharp;

var cartesianChart = new SKCartesianChart
{
    Width = 900,
    Height = 600,
    Series = new ISeries[]
    {
        new LineSeries<int> { Values = new int[] { 1, 5, 4, 6 } },
        new ColumnSeries<int> { Values = new int[] { 4, 8, 2, 4 } }
    },
    Title = new LabelVisual
    {
        Text = "Hello LiveCharts",
        TextSize = 30,
        Padding = new Padding(15),
        Paint = new SolidColorPaint(0xff303030)
    },
    LegendPosition = LiveChartsCore.Measure.LegendPosition.Right,
    Background = SKColors.White
};

// you can save the image to png (by default)
// or use the second argument to specify another format.
cartesianChart.SaveImage("cartesianChart.png");

var pieChart = new SKPieChart
{
    Width = 900,
    Height = 600,
    Series = new ISeries[]
    {
        new PieSeries<int> { Values = new int[] { 10, } },
        new PieSeries<int> { Values = new int[] { 6 } },
        new PieSeries<int> { Values = new int[] { 4 } }
    },
    LegendPosition = LiveChartsCore.Measure.LegendPosition.Right
};

pieChart.SaveImage("pieChart.png");

var geoHeatMap = new SKGeoMap
{
    Width = 900,
    Height = 600,
    Series = new IGeoSeries[]
    {
        new HeatLandSeries
        {
            Lands = new HeatLand[]
            {
                new() { Name = "mex", Value = 10 },
                new() { Name = "usa", Value = 15 },
                new() { Name = "can", Value = 8 }
            }
        }
    }
};

geoHeatMap.SaveImage("geoHeatMap.png");

// alternatively you can get the image and do different operations:
using var image = cartesianChart.GetImage();
using var data = image.Encode();
var base64CartesianChart = Convert.ToBase64String(data.AsSpan());

Console.WriteLine("Images saved at the root folder!");
