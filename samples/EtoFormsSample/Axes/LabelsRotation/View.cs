using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.Axes.LabelsRotation;

namespace EtoFormsSample.Axes.LabelsRotation;

public class View : Panel
{
    private readonly CartesianChart _cartesianChart;
    private readonly ViewModel _viewModel;

    public View()
    {
        _viewModel = new ViewModel();

        _cartesianChart = new CartesianChart
        {
            Series = _viewModel.Series,
            YAxes = _viewModel.YAxes,
        };

        var b1 = new Slider() { Width = 300, MinValue = -360, MaxValue = 720 };
        b1.ValueChanged += (object sender, System.EventArgs e) =>
        {
            _viewModel.YAxes[0].LabelsRotation = b1.Value;
        };

        Content = new DynamicLayout(new StackLayout(b1), _cartesianChart);
    }
}
