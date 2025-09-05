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

        Loaded += (_, _) =>
        {
            // in this case in the constructor of this view // mark
            // we render our chart controls as images // mark
            CreateImageFromCartesianControl();
            CreateImageFromPieControl();
        };
    }

    private void CreateImageFromCartesianControl()
    {
        // you can take any chart in the UI, and build an image from it // mark
        var chartControl = this.FindControl<CartesianChart>("cartesianChart")!;
        var skChart = new SKCartesianChart(chartControl);
        skChart.SaveImage("CartesianImageFromControl.png");
    }

    private void CreateImageFromPieControl()
    {
        var chartControl = this.FindControl<PieChart>("pieChart")!;
        var skChart = new SKPieChart(chartControl);
        skChart.SaveImage("PieImageFromControl.png");
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
