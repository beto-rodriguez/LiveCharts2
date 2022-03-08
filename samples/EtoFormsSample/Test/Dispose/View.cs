using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.Test.Dispose;

namespace EtoFormsSample.Test.Dispose;

public class View : Panel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="View"/> class.
    /// </summary>
    public View()
    {
        var button = new Button() { Text = "Change Data", Height = 40 };

        button.Click += (o, e) => { Content = createLayout(button); };

        Content = createLayout(button);
    }

    private static Control createLayout(Button button)
    {
        var viewModel = new ViewModel();

        return new DynamicLayout(
            new DynamicRow(new DynamicControl()
            {
                Control = button,
                XScale = true
            }),
            new DynamicRow(new DynamicControl()
            {
                Control = new CartesianChart() { Series = viewModel.CartesianSeries },
                YScale = true
            }),
            new DynamicRow(new DynamicControl()
            {
                Control = new PieChart() { Series = viewModel.PieSeries },
                YScale = true
            }),
            new DynamicRow(new DynamicControl()
            {
                Control = new PolarChart() { Series = viewModel.PolarSeries },
                YScale = true
            }),
            new DynamicRow(new DynamicControl()
            {
                Control = new GeoMap() { Series = viewModel.GeoSeries },
                YScale = true
            })
            );
    }
}
