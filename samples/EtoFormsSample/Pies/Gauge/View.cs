using Eto.Forms;
using Eto.Drawing;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.Pies.Gauge;

namespace EtoFormsSample.Pies.Gauge;

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

        var slider1 = new Slider() { MinValue = -360, MaxValue = 720 };
        _ = slider1.ValueBinding.Bind(viewModel, v => (int)v.InitialRotation);
        var slider2 = new Slider() { MinValue = 0, MaxValue = 360 };
        _ = slider2.ValueBinding.Bind(viewModel, v => (int)v.MaxAngle);
        var slider3 = new Slider() { MinValue = 0, MaxValue = 50 };
        _ = slider3.ValueBinding.Bind(viewModel, v => (int)v.InnerRadius);
        var slider4 = new Slider() { MinValue = 0, MaxValue = 50 };
        _ = slider4.ValueBinding.Bind(viewModel, v => (int)v.OffsetRadius);
        var slider5 = new Slider() { MinValue = 0, MaxValue = 50 };
        _ = slider5.ValueBinding.Bind(viewModel, v => (int)v.BackgroundInnerRadius);
        var slider6 = new Slider() { MinValue = 0, MaxValue = 50 };
        _ = slider6.ValueBinding.Bind(viewModel, v => (int)v.BackgroundOffsetRadius);

        var sliders = new DynamicLayout(
            null,
            new Label { Text = "Initial rotation" }, slider1,
            new Label { Text = "Max angle" }, slider2,
            new Label { Text = "Inner radius" }, slider3,
            new Label { Text = "Offset radius" }, slider4,
            new Label { Text = "Background inner radius" }, slider5,
            new Label { Text = "Background offset radius" }, slider6,
            null
            )
        { BackgroundColor = Colors.White };

        Content = new DynamicLayout(new DynamicRow(sliders, pieChart));
    }
}
