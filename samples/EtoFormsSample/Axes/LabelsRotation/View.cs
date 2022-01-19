using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto.Forms;
using ViewModelsSamples.Axes.LabelsRotation;

namespace EtoFormsSample.Axes.LabelsRotation;

public class View : Panel
{
    private readonly CartesianChart _cartesianChart;
    private readonly ViewModel _viewModel;

    public View()
    {
        InitializeComponent();
        Size = new Eto.Drawing.Size(100, 100);

        _viewModel = new ViewModel();

        _cartesianChart = new CartesianChart
        {
            Series = _viewModel.Series,
            YAxes = _viewModel.YAxes,

            // out of livecharts properties...
            Location = new Eto.Drawing.Point(0, 50),
            Size = new Eto.Drawing.Size(100, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(_cartesianChart);

        var b1 = new TrackBar { Location = new Eto.Drawing.Point(0, 0), Width = 300, Minimum = -360, Maximum = 720 };
        b1.ValueChanged += (object sender, System.EventArgs e) =>
        {
            _viewModel.YAxes[0].LabelsRotation = b1.Value;
        };

        Controls.Add(b1);
    }
}
