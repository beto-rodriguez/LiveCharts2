// NOTE: // mark
// BECAUSE THIS VIEWMODEL IS SHARED WITH OTHER VIEWS // mark
// THE _viewModel.ChartUpdated, _viewModel.PointerDown and _viewModel.PointerUp METHODS // mark
// are repeated in WindowsForms, WindowsForms do not support Command binding, please // mark
// ignore the viewmodel RelayCommands and use the events instead. // mark

using System.Linq;
using System.Windows.Forms;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.WinForms;
using ViewModelsSamples.General.Scrollable;

namespace WinFormsSample.General.Scrollable;

public partial class View : UserControl
{
    private readonly ViewModel _viewModel = new();
    private readonly CartesianChart _scrollBarChart;
    private bool _isDown = false;

    public View()
    {
        InitializeComponent();

        Size = new System.Drawing.Size(50, 50);

        var viewModel = _viewModel;

        var cartesianChart = new CartesianChart
        {
            Series = viewModel.Series,
            XAxes = viewModel.ScrollableAxes,
            ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.X,
            DrawMargin = viewModel.Margin,

            // out of livecharts properties...
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Dock = DockStyle.Fill
        };

        cartesianChart.UpdateStarted += OnChart_Updated;

        var cartesianChart2 = new CartesianChart
        {
            Series = viewModel.ScrollbarSeries,
            DrawMargin = viewModel.Margin,
            Sections = viewModel.Thumbs,
            XAxes = viewModel.InvisibleX,
            YAxes = viewModel.InvisibleY,
            TooltipPosition = LiveChartsCore.Measure.TooltipPosition.Hidden,

            // out of livecharts properties...
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Dock = DockStyle.Fill
        };

        _scrollBarChart = cartesianChart2;

        cartesianChart2.MouseDown += CartesianChart2_MouseDown;
        cartesianChart2.GetDrawnControl().MouseMove += CartesianChart2_MouseMove;
        cartesianChart2.MouseUp += CartesianChart2_MouseUp;

        var splitContainer = new SplitContainer
        {
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            //IsSplitterFixed = true,
            Orientation = Orientation.Horizontal
        };

        splitContainer.Panel1.Controls.Add(cartesianChart);
        splitContainer.Panel2.Controls.Add(cartesianChart2);
        Controls.Add(splitContainer);
    }

    private void OnChart_Updated(IChartView<SkiaSharpDrawingContext> chart)
    {
        var vm = _viewModel;
        var cartesianChart = (CartesianChart)chart;

        var x = cartesianChart.XAxes.First();

        // update the scroll bar thumb when the chart is updated (zoom/pan)
        // this will let the user know the current visible range
        var thumb = vm.Thumbs[0];

        thumb.Xi = x.MinLimit;
        thumb.Xj = x.MaxLimit;
    }

    private void CartesianChart2_MouseDown(object? sender, MouseEventArgs e)
    {
        _isDown = true;
    }

    private void CartesianChart2_MouseMove(object? sender, MouseEventArgs e)
    {
        if (!_isDown) return;

        var vm = _viewModel;
        var scrollBarChart = _scrollBarChart;

        var positionInData = scrollBarChart.ScalePixelsToData(new(e.Location.X, e.Location.Y));

        var thumb = vm.Thumbs[0];
        var currentRange = thumb.Xj - thumb.Xi;

        // update the scroll bar thumb when the user is dragging the chart
        thumb.Xi = positionInData.X - currentRange / 2;
        thumb.Xj = positionInData.X + currentRange / 2;

        // update the chart visible range
        vm.ScrollableAxes[0].MinLimit = thumb.Xi;
        vm.ScrollableAxes[0].MaxLimit = thumb.Xj;
    }

    private void CartesianChart2_MouseUp(object? sender, MouseEventArgs e)
    {
        _isDown = false;
    }
}
