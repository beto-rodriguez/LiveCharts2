using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore.VisualElements;
using ViewModelsSamples.General.VisualElements;

namespace WinFormsSample.General.VisualElements;

public partial class View : UserControl
{
    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        var viewModel = new ViewModel();

        var cartesianChart = new CartesianChart
        {
            Series = viewModel.Series,
            VisualElements = viewModel.VisualElements,
            TooltipPosition = LiveChartsCore.Measure.TooltipPosition.Hidden,

            // out of livecharts properties...
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        cartesianChart.VisualElementsPointerDown += CartesianChart_VisualElementsPointerDown;

        Controls.Add(cartesianChart);
    }

    private void CartesianChart_VisualElementsPointerDown(
        IChartView chart, IEnumerable<VisualElement<SkiaSharpDrawingContext>> visualElements)
    {
        // the visualElements contains all the elements that were clicked.

        foreach (var visual in visualElements)
        {
            visual.X++;
        }
    }
}
