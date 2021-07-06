using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LiveChartsCore.SkiaSharpView.Avalonia;
using LiveChartsCore.SkiaSharpView.SKCharts;
using ViewModelsSamples.General.ChartToImage;

namespace AvaloniaSample.General.ChartToImage
{
    public class View : UserControl
    {
        public View()
        {
            InitializeComponent();

            var vm = new ViewModel();
            DataContext = vm;

            // you can create a image of a chart from memory using the
            // SKCartesianChart, SKPieChart or SKGeoMap controls.
            var skChart = new SKCartesianChart
            {
                Width = 900,
                Height = 600,
                Series = vm.Series
            };

            skChart.SaveImage("chartImageFromMemory.png");

            // or you could also use a chart in the user interface to create an image
            CreateImageFromControl();
        }

        private void CreateImageFromControl()
        {
            var chartControl = this.FindControl<CartesianChart>("chart");
            var skChart = new SKCartesianChart(chartControl) { Width = 900, Height = 600, };
            skChart.SaveImage("chartImageFromControl.png");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
