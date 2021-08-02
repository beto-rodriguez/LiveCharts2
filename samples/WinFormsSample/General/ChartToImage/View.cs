using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.SkiaSharpView.WinForms;
using System;
using System.Windows.Forms;
using ViewModelsSamples.General.ChartToImage;

namespace WinFormsSample.General.ChartToImage
{
    public partial class View : UserControl
    {
        private CartesianChart _cartesian;
        private PieChart _pie;
        private GeoMap _map;

        public View()
        {
            InitializeComponent();
            Size = new System.Drawing.Size(90, 90);

            var viewModel = new ViewModel();


            // Adding a cartesian chart to the UI...
            _cartesian = new CartesianChart
            {
                Series = viewModel.CatesianSeries,

                // out of livecharts properties...
                Location = new System.Drawing.Point(0, 0),
                Size = new System.Drawing.Size(400, 200),
                //Anchor = AnchorStyles.Left | AnchorStyles.Right
            };
            Controls.Add(_cartesian);

            // Adding a pie chart to the UI...
            _pie = new PieChart
            {
                Series = viewModel.PieSeries,

                // out of livecharts properties...
                Location = new System.Drawing.Point(0, 200),
                Size = new System.Drawing.Size(400, 200),
                //Anchor = AnchorStyles.Left | AnchorStyles.Right
            };
            Controls.Add(_pie);

            // Adding a map chart to the UI...
            _map = new GeoMap
            {
                Values = viewModel.MapValues,

                // out of livecharts properties...
                Location = new System.Drawing.Point(0, 400),
                Size = new System.Drawing.Size(400, 200),
                //Anchor = AnchorStyles.Left | AnchorStyles.Right
            };
            Controls.Add(_map);

            // CARTESIAN CHART IMAGE

            // you can create an image of a chart from memory using the
            // SKCartesianChart, SKPieChart or SKGeoMap classes.

            // in the case of this sample
            // the image was generated at the root folder
            var cartesianChart = new SKCartesianChart
            {
                Width = 900,
                Height = 600,
                Series = viewModel.CatesianSeries
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
            image.Dispose();

            // or you could also use a chart in the user interface to create an image
            CreateImageFromCartesianControl();


            // PIE CHART IMAGE
            new SKPieChart
            {
                Width = 900,
                Height = 600,
                Series = viewModel.PieSeries
            }.SaveImage("PieImageFromMemory.png");

            // or create it from a control in the UI
            CreateImageFromPieControl();


            // GEO MAP CHART IMAGE
            new SKGeoMap
            {
                Width = 900,
                Height = 600,
                Values = viewModel.MapValues
            }.SaveImage("MapImageFromMemory.png");

            // or create it from a control in the UI
            CreateImageFromGeoControl();
        }

        private void CreateImageFromCartesianControl()
        {
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
}
