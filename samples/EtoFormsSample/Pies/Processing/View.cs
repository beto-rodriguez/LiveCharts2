using System.Drawing;
using System.Windows.Forms;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WinForms;
using ViewModelsSamples.Pies.Processing;

namespace WinFormsSample.Pies.Processing;

public partial class View : UserControl
{
    private readonly PieChart pieChart;

    public View()
    {
        InitializeComponent();
        Size = new Size(100, 100);

        var viewModel = new ViewModel();

        pieChart = new PieChart
        {
            Series = viewModel.Series,

            // out of livecharts properties...
            Location = new Point(0, 50),
            Size = new Size(100, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(pieChart);

        var b1 = new Label { Location = new Point(0, 0) };
        Controls.Add(b1);

        var b2 = new Label { Location = new Point(100, 0) };
        Controls.Add(b2);

        var b3 = new Label { Location = new Point(200, 0) };
        Controls.Add(b3);

        pieChart.UpdateStarted += chart =>
        {
            var series = (PieSeries<ObservableValue>)viewModel.Series[0];
            var values = (ObservableValue[])series.Values;
            b1.Text = values[0].Value + " " + series.Name;
            b1.ForeColor = GetForeColor(series);

            series = (PieSeries<ObservableValue>)viewModel.Series[1];
            values = (ObservableValue[])series.Values;
            b2.Text = values[0].Value + " " + series.Name;
            b2.ForeColor = GetForeColor(series);

            series = (PieSeries<ObservableValue>)viewModel.Series[2];
            values = (ObservableValue[])series.Values;
            b3.Text = values[0].Value + " " + series.Name;
            b3.ForeColor = GetForeColor(series);
        };
    }

    private static Color GetForeColor(PieSeries<ObservableValue> pieSeries)
    {
        return pieSeries.Fill is not SolidColorPaint solidColorBrush
            ? new Color()
            : Color.FromArgb(
                solidColorBrush.Color.Alpha,
                solidColorBrush.Color.Red,
                solidColorBrush.Color.Green,
                solidColorBrush.Color.Blue);
    }
}
