using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.Eto;

namespace EtoFormsSample.Pies.Gauge1;

public class View : Panel
{
    private readonly PieChart pieChart;

    public View()
    {
        pieChart = new PieChart
        {
            Series = GaugeGenerator.BuildSolidGauge(
                new GaugeItem(
                    30,
                    series =>
                    {
                        series.MaxRadialColumnWidth = 50;
                        series.DataLabelsSize = 50;
                    })),
            InitialRotation = -90,
            MinValue = 0,
            MaxValue = 100
        };

        Content = pieChart;
    }
}
