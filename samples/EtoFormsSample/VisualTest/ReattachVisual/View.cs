using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto.Forms;
using ViewModelsSamples.VisualTest.ReattachVisual;

namespace EtoFormsSample.VisualTest.ReattachVisual;

public class View : Panel
{
    private bool _isInVisualTree = true;
    private readonly CartesianChart _cartesianChart;

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

        var b = new Button
        {
            Text = "Toggle"
        };
        b.Click += B_Click;

        Content = new StackLayout(b, _cartesianChart);
    }

    private void B_Click(object sender, System.EventArgs e)
    {
        if (_isInVisualTree)
        {
            (Content as StackLayout).Items.RemoveAt(1);
            _isInVisualTree = false;
            return;
        }

        (Content as StackLayout).Items.Add(_cartesianChart);
        _isInVisualTree = true;
    }
}
