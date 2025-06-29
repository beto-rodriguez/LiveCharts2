using System.Windows.Forms;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WinForms;

namespace WinFormsSample.Box.Basic;

public partial class View : UserControl
{
    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        var values1 = new BoxValue[]
        {
            new(100, 80, 60, 20, 70),
            new(90, 70, 50, 30, 60),
            new(80, 60, 40, 10, 50)
        };
        var values2 = new BoxValue[]
        {
            new(90, 70, 50, 30, 60),
            new(80, 60, 40, 10, 50),
            new(70, 50, 30, 20, 40)
        };
        var values3 = new BoxValue[]
        {
            new(80, 60, 40, 10, 50),
            new(70, 50, 30, 20, 40),
            new(60, 40, 20, 10, 30)
        };
        var labels = new string[] { "Apperitizers", "Mains", "Desserts" };

        var series = new ISeries[]
        {
            new BoxSeries<BoxValue> { Name = "Year 2023", Values = values1 },
            new BoxSeries<BoxValue> { Name = "Year 2024", Values = values2 },
            new BoxSeries<BoxValue> { Name = "Year 2025", Values = values3 }
        };

        var xAxis = new Axis
        {
            Labels = labels,
            LabelsRotation = 0,
            SeparatorsPaint = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint(new SkiaSharp.SKColor(200, 200, 200)),
            SeparatorsAtCenter = false,
            TicksPaint = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint(new SkiaSharp.SKColor(35, 35, 35)),
            TicksAtCenter = true
        };

        var cartesianChart = new CartesianChart
        {
            Series = series,
            XAxes = [xAxis],
            LegendPosition = LiveChartsCore.Measure.LegendPosition.Right,
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(cartesianChart);
    }
}
