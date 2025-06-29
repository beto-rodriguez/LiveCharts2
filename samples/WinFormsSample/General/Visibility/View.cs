using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;

namespace WinFormsSample.General.Visibility;

public partial class View : UserControl
{
    private readonly CartesianChart cartesianChart;
    private readonly ISeries[] series;

    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(100, 100);

        var values0 = new double[] { 2, 5, 4, 3 };
        var values1 = new double[] { 1, 2, 3, 4 };
        var values2 = new double[] { 4, 3, 2, 1 };
        var isVisible = new bool[] { true, true, true };

        series = [
            new ColumnSeries<double> { Values = values0, IsVisible = isVisible[0] },
            new ColumnSeries<double> { Values = values1, IsVisible = isVisible[1] },
            new ColumnSeries<double> { Values = values2, IsVisible = isVisible[2] }
        ];

        cartesianChart = new CartesianChart
        {
            Series = series,
            Location = new System.Drawing.Point(0, 50),
            Size = new System.Drawing.Size(100, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };
        Controls.Add(cartesianChart);

        var b1 = new Button { Text = "toggle 1", Location = new System.Drawing.Point(0, 0) };
        b1.Click += (sender, e) => ToggleSeries(0);
        Controls.Add(b1);

        var b2 = new Button { Text = "toggle 2", Location = new System.Drawing.Point(80, 0) };
        b2.Click += (sender, e) => ToggleSeries(1);
        Controls.Add(b2);

        var b3 = new Button { Text = "toggle 3", Location = new System.Drawing.Point(160, 0) };
        b3.Click += (sender, e) => ToggleSeries(2);
        Controls.Add(b3);
    }

    private void ToggleSeries(int index)
    {
        if (series[index] is ISeries s)
        {
            s.IsVisible = !s.IsVisible;
            cartesianChart.Update();
        }
    }
}
