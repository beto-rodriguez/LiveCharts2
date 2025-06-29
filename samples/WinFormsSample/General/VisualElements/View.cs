using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore.Kernel;
using ViewModelsSamples.General.VisualElements;

namespace WinFormsSample.General.VisualElements;

public partial class View : UserControl
{
    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(600, 400);

        var visualElements = new IChartElement[]
        {
            new RectangleVisual(),
            new ScaledRectangleVisual(),
            new PointerDownAwareVisual(),
            new SvgVisual(),
            new ThemedVisual(),
            new CustomVisual(),
            new AbsoluteVisual(),
            new StackedVisual(),
            new TableVisual(),
            new ContainerVisual(),
        };

        var cartesianChart = new CartesianChart
        {
            VisualElements = visualElements,
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(600, 400),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(cartesianChart);
    }
}
