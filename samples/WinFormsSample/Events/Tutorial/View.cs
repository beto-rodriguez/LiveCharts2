using System.Collections.Generic;
using System.Windows.Forms;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.WinForms;
using ViewModelsSamples.Events.Tutorial;

namespace WinFormsSample.Events.Tutorial;

public partial class View : UserControl
{
    private readonly ViewModel _viewModel;
    private readonly CartesianChart _chart;

    /// <summary>
    /// Initializes a new instance of the <see cref="View"/> class.
    /// </summary>
    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        _viewModel = new ViewModel();

        _chart = new CartesianChart
        {
            Series = _viewModel.SeriesCollection,
            FindingStrategy = _viewModel.Strategy,

            // out of livecharts properties...
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        _chart.MouseDown += CartesianChart_MouseDown;
        _chart.HoveredPointsChanged += OnHoveredChanged;

        Controls.Add(_chart);
    }

    private void CartesianChart_MouseDown(object sender, MouseEventArgs e) =>
        _viewModel.OnPressed(new PointerCommandArgs(_chart, new(e.Location.X, e.Location.Y), e));

    private void OnHoveredChanged(
        IChartView chart, IEnumerable<ChartPoint> newItems, IEnumerable<ChartPoint> oldItems) =>
            _viewModel.OnHoveredPointsChanged(new HoverCommandArgs(chart, newItems, oldItems));
}
