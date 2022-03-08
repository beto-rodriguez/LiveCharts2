using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.VisualTest.ReattachVisual;

namespace EtoFormsSample.VisualTest.ReattachVisual;

public class View : Panel
{
    private bool _isInVisualTree = true;
    private CartesianChart _cartesianChart;
    private Button _toggleButton;

    /// <summary>
    /// Initializes a new instance of the <see cref="View"/> class.
    /// </summary>
    public View()
    {
        var viewModel = new ViewModel();

        _cartesianChart = new CartesianChart
        {
            Series = viewModel.Series,
        };

        _toggleButton = new Button
        {
            Text = "Toggle"
        };
        _toggleButton.Click += toggleButton_Click;

        UpdateLayout();
    }

    void UpdateLayout()
    {
        var chart = _isInVisualTree ? _cartesianChart : null;

        var chartRow = new DynamicRow(new DynamicControl() { Control = chart, YScale = true });

        Content = new DynamicLayout(chartRow, _toggleButton);
    }
    private void toggleButton_Click(object sender, System.EventArgs e)
    {
        _isInVisualTree = !_isInVisualTree;

        UpdateLayout();
    }
}
