using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.WinForms;
using ViewModelsSamples.Events.Tutorial;

namespace WinFormsSample.Events.Tutorial;

public partial class View : UserControl
{
    private readonly ViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="View"/> class.
    /// </summary>
    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        _viewModel = new ViewModel();

        var cartesianChart = new CartesianChart
        {
            Series = _viewModel.SeriesCollection,
            FindingStrategy = _viewModel.Strategy,

            // out of livecharts properties...
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        cartesianChart.DataPointerDown += CartesianChart_DataPointerDown;

        Controls.Add(cartesianChart);
    }

    private void CartesianChart_DataPointerDown(IChartView chart, IEnumerable<ChartPoint> points)
        => _viewModel.OnDataDown(points);
}
