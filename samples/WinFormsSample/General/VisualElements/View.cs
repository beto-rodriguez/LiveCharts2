using System.Windows.Forms;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.WinForms;
using ViewModelsSamples.General.VisualElements;

namespace WinFormsSample.General.VisualElements;

public partial class View : UserControl
{
    private readonly ContextMenuStrip _fruitContextMenuStrip;

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

        _fruitContextMenuStrip = new ContextMenuStrip();
        _fruitContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(cms_Opening);
        var ts = new ToolStrip();
        var fruitToolStripDropDownButton = new ToolStripDropDownButton("Fruit", null, null, "Fruit");
        _ = ts.Items.Add(fruitToolStripDropDownButton);
        ts.Dock = DockStyle.Top;
        fruitToolStripDropDownButton.DropDown = _fruitContextMenuStrip;
        var ms = new MenuStrip();
        var fruitToolStripMenuItem = new ToolStripMenuItem("Fruit", null, null, "Fruit");
        _ = ms.Items.Add(fruitToolStripMenuItem);
        ms.Dock = DockStyle.Top;
        fruitToolStripMenuItem.DropDown = _fruitContextMenuStrip;
        ContextMenuStrip = _fruitContextMenuStrip;
        Controls.Add(ts);
        var b = new Button { Location = new System.Drawing.Point(60, 60) };
        Controls.Add(b);
        b.ContextMenuStrip = _fruitContextMenuStrip;
        Controls.Add(ms);
    }

    private void cms_Opening(object sender, System.ComponentModel.CancelEventArgs e)
    {
        // Acquire references to the owning control and item.
        var c = _fruitContextMenuStrip.SourceControl;

        // Clear the ContextMenuStrip control's Items collection.
        _fruitContextMenuStrip.Items.Clear();

        // Check the source control first.
        if (c != null)
        {
            // Add custom item (Form)
            _ = _fruitContextMenuStrip.Items.Add("Source: " + c.GetType().ToString());
        }
        else if (_fruitContextMenuStrip.OwnerItem is ToolStripDropDownItem tsi)
        {
            // Add custom item (ToolStripDropDownButton or ToolStripMenuItem)
            _ = _fruitContextMenuStrip.Items.Add("Source: " + tsi.GetType().ToString());
        }

        // Populate the ContextMenuStrip control with its default items.
        _ = _fruitContextMenuStrip.Items.Add("-");
        _ = _fruitContextMenuStrip.Items.Add("Apples");
        _ = _fruitContextMenuStrip.Items.Add("Oranges");
        _ = _fruitContextMenuStrip.Items.Add("Pears");

        // Set Cancel to false. 
        // It is optimized to true based on empty entry.
        e.Cancel = false;
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
