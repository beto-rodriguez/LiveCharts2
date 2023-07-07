using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.WPF;
using ViewModelsSamples.General.Scrollable;

namespace WPFSample.General.Scrollable;

/// <summary>
/// Interaction logic for View.xaml
/// </summary>
public partial class View : UserControl
{
    private bool _isDown = true;

    public View()
    {
        InitializeComponent();
    }

    private void OnChart_Updated(IChartView<SkiaSharpDrawingContext> chart)
    {
        var vm = (ViewModel)DataContext;
        var cartesianChart = (CartesianChart)chart;

        var x = cartesianChart.XAxes.First();

        var thumb = vm.Thumbs[0];
        thumb.Xi = x.MinLimit;
        thumb.Xj = x.MaxLimit;
    }

    private void CartesianChart_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _isDown = true;
    }

    private void CartesianChart_MouseMove(object sender, MouseEventArgs e)
    {
        if (!_isDown) return;

        var vm = (ViewModel)DataContext;
        var scrollBarChart = (CartesianChart)FindName("ScrollBarChart");

        var pointerPosition = e.GetPosition(scrollBarChart);
        var positionInData = scrollBarChart.ScalePixelsToData(new(pointerPosition.X, pointerPosition.Y));

        var thumb = vm.Thumbs[0];
        var currentRange = thumb.Xj - thumb.Xi;

        thumb.Xi = positionInData.X - currentRange / 2;
        thumb.Xj = positionInData.X + currentRange / 2;

        vm.ScrollableAxes[0].MinLimit = thumb.Xi;
        vm.ScrollableAxes[0].MaxLimit = thumb.Xj;
    }

    private void CartesianChart_MouseUp(object sender, MouseButtonEventArgs e)
    {
        _isDown = false;
    }
}
