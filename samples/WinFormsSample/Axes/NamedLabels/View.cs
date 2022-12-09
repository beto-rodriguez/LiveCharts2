using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WinForms;
using SkiaSharp;
using ViewModelsSamples.Axes.NamedLabels;

namespace WinFormsSample.Axes.NamedLabels;

public partial class View : UserControl
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        var viewModel = new ViewModel();

        cartesianChart = new CartesianChart
        {
            Series = viewModel.Series,
            XAxes = viewModel.XAxes,
            YAxes = viewModel.YAxes,
            TooltipTextSize = 16, // mark
            TooltipTextPaint = viewModel.TooltipTextPaint, // mark
            TooltipBackgroundPaint = viewModel.TooltipBackgroundPaint, // mark
            TooltipPosition = LiveChartsCore.Measure.TooltipPosition.Left, // mark

            // out of livecharts properties...
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(cartesianChart);
    }
}
