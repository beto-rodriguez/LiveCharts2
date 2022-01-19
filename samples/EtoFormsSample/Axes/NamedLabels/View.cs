using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto.Forms;
using ViewModelsSamples.Axes.NamedLabels;

namespace EtoFormsSample.Axes.NamedLabels;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        InitializeComponent();
        Size = new Eto.Drawing.Size(50, 50);

        var viewModel = new ViewModel();

        cartesianChart = new CartesianChart
        {
            Series = viewModel.Series,
            XAxes = viewModel.XAxes,
            YAxes = viewModel.YAxes,
            TooltipPosition = LiveChartsCore.Measure.TooltipPosition.Left, // mark
            TooltipFont = new Eto.Drawing.Font("Courier New", 25), // mark
            TooltipTextColor = Eto.Drawing.Color.FromArgb(255, 242, 244, 195), // mark
            TooltipBackColor = Eto.Drawing.Color.FromArgb(255, 72, 0, 50), // mark

            // out of livecharts properties...
            Location = new Eto.Drawing.Point(0, 0),
            Size = new Eto.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(cartesianChart);
    }
}
