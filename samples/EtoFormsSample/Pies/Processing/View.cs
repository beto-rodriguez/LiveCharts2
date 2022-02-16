using Eto.Drawing;
using Eto.Forms;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.Pies.Processing;

namespace EtoFormsSample.Pies.Processing;

public class View : Panel
{
    private readonly PieChart pieChart;

    public View()
    {
        var viewModel = new ViewModel();

        pieChart = new PieChart
        {
            Series = viewModel.Series,
        };

        var b1 = new Label();
        var b2 = new Label();
        var b3 = new Label();

        pieChart.UpdateStarted += chart =>
        {
            var series = (PieSeries<ObservableValue>)viewModel.Series[0];
            var values = (ObservableValue[])series.Values;
            b1.Text = values[0].Value + " " + series.Name;
            b1.TextColor = GetForeColor(series);

            series = (PieSeries<ObservableValue>)viewModel.Series[1];
            values = (ObservableValue[])series.Values;
            b2.Text = values[0].Value + " " + series.Name;
            b2.TextColor = GetForeColor(series);

            series = (PieSeries<ObservableValue>)viewModel.Series[2];
            values = (ObservableValue[])series.Values;
            b3.Text = values[0].Value + " " + series.Name;
            b3.TextColor = GetForeColor(series);
        };

        var buttons = new StackLayout(b1, b2, b3) { Orientation = Orientation.Horizontal, Padding = 2, Spacing = 4 };

        Content = new DynamicLayout(buttons, pieChart);
    }

    private static Color GetForeColor(PieSeries<ObservableValue> pieSeries)
    {
        return pieSeries.Fill is not SolidColorPaint solidColorBrush
            ? new Color()
            : Color.FromArgb(
                solidColorBrush.Color.Red,
                solidColorBrush.Color.Green,
                solidColorBrush.Color.Blue,
                solidColorBrush.Color.Alpha);
    }
}
