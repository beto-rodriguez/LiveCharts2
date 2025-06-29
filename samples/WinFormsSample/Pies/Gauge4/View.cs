using System.Windows.Forms;
using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.WinForms;

namespace WinFormsSample.Pies.Gauge4;

public partial class View : UserControl
{
    private readonly PieChart pieChart;

    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        pieChart = new PieChart
        {
            Series = GaugeGenerator.BuildSolidGauge(
                new GaugeItem(50, series => SetStyle("Vanessa", series)),
                new GaugeItem(80, series => SetStyle("Charles", series)),
                new GaugeItem(95, series => SetStyle("Ana", series)),
                new GaugeItem(GaugeItem.Background, series =>
                {
                    series.Fill = null;
                })),
            InitialRotation = -90,
            MaxAngle = 350,
            MinValue = 0,
            MaxValue = 100,
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(pieChart);
    }

    public static void SetStyle(string name, PieSeries<ObservableValue> series)
    {
        series.Name = name;
        series.DataLabelsSize = 20;
        series.DataLabelsPosition = PolarLabelsPosition.End;
        series.DataLabelsFormatter =
                point => point.Coordinate.PrimaryValue.ToString();
        series.InnerRadius = 20;
        series.MaxRadialColumnWidth = 5;
    }
}
