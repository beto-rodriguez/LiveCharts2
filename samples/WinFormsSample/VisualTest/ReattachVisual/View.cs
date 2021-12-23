using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.WinForms;
using ViewModelsSamples.VisualTest.ReattachVisual;

namespace WinFormsSample.VisualTest.ReattachVisual;

public partial class View : UserControl
{
    private bool _isInVisualTree = true;
    private readonly CartesianChart _cartesianChart;

    /// <summary>
    /// Initializes a new instance of the <see cref="View"/> class.
    /// </summary>
    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        var viewModel = new ViewModel();

        _cartesianChart = new CartesianChart
        {
            Series = viewModel.Series,

            // out of livecharts properties...
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        var b = new Button
        {
            Size = new System.Drawing.Size(150, 50),
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
