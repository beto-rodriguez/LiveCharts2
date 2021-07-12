using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.SkiaSharpView.WinForms;
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
                Series = viewModel.CaterianSeries,

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

            // you can create a image of a chart from memory using the
            // SKCartesianChart, SKPieChart or SKGeoMap controls.

            // in the case of this sample
            // the image was generated at the root folder ( samples/AvaloniaSample/bin/Debug/{targetFramework}/ )
            new SKCartesianChart
            {
                Width = 900,
                Height = 600,
                Series = viewModel.CaterianSeries
            }.SaveImage("CartesianImageFromMemory.png"); // <- path where the image will be generated

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
