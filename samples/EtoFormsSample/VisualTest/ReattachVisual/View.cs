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
        InitializeComponent();
        Size = new Eto.Drawing.Size(50, 50);

        var viewModel = new ViewModel();

        _cartesianChart = new CartesianChart
        {
            Series = viewModel.Series,

            // out of livecharts properties...
            Location = new Eto.Drawing.Point(0, 0),
            Size = new Eto.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        var b = new Button
        {
            Size = new Eto.Drawing.Size(150, 50),
            Text = "Toggle"
        };
        b.Click += B_Click;

        Controls.Add(_cartesianChart);
        Controls.Add(b);
        b.BringToFront();
    }

    private void B_Click(object sender, System.EventArgs e)
    {
        if (_isInVisualTree)
        {
            Controls.Remove(_cartesianChart);
            _isInVisualTree = false;
            return;
        }

        Controls.Add(_cartesianChart);
        _isInVisualTree = true;
    }
}
