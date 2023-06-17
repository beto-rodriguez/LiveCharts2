using System.Windows.Forms;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.WinForms;
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
            ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.X,

            // out of livecharts properties...
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        cartesianChart.VisualElementsPointerDown += CartesianChart_VisualElementsPointerDown;

        Controls.Add(cartesianChart);
    }

    private void CartesianChart_VisualElementsPointerDown(
        IChartView chart, VisualElementsEventArgs<SkiaSharpDrawingContext> visualElementsArgs)
    {
        if (visualElementsArgs.ClosestToPointerVisualElement is null) return;
        visualElementsArgs.ClosestToPointerVisualElement.X++;

        // alternatively you can use the visual elements collection.
        //foreach (var visualElement in visualElementsArgs.VisualElements)
        //{
        //    visualElement.X++;
        //}
    }
}
