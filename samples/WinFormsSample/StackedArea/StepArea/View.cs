using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;

namespace WinFormsSample.StackedArea.StepArea;

public partial class View : UserControl
{
    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        var values1 = new double[] { 3, 2, 3, 5, 3, 4, 6 };
        var values2 = new double[] { 6, 5, 6, 3, 8, 5, 2 };
        var values3 = new double[] { 4, 8, 2, 8, 9, 5, 3 };

        var series = new ISeries[]
        {
            new StackedStepAreaSeries<double> { Values = values1 },
            new StackedStepAreaSeries<double> { Values = values2 },
            new StackedStepAreaSeries<double> { Values = values3 }
        };

        var cartesianChart = new CartesianChart
        {
            Series = series,
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(cartesianChart);
    }
}
