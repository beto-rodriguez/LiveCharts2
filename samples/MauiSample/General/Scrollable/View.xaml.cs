using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Maui;
using ViewModelsSamples.General.Scrollable;

namespace MauiSample.General.Scrollable;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class View : ContentPage
{
    private bool _isDown = true;

    public View()
    {
        InitializeComponent();
    }

    private void OnChart_Updated(IChartView<SkiaSharpDrawingContext> chart)
    {
        var vm = (ViewModel)BindingContext;
        var cartesianChart = (CartesianChart)chart;

        var x = cartesianChart.XAxes.First();

        // update the scroll bar thumb when the chart is updated (zoom/pan)
        // this will let the user know the current visible range
        var thumb = vm.Thumbs[0];

        thumb.Xi = x.MinLimit;
        thumb.Xj = x.MaxLimit;
    }

    private void OnPointerEntered(object sender, PointerEventArgs e)
    {
        _isDown = true;
    }

    private void OnPointerMoved(object sender, PointerEventArgs e)
    {
        if (!_isDown) return;

        var vm = (ViewModel)BindingContext;
        var scrollBarChart = ScrollBarChart;

        var pointerPosition = e.GetPosition(scrollBarChart);
        var positionInData = scrollBarChart.ScalePixelsToData(new(pointerPosition.Value.X, pointerPosition.Value.Y));

        var thumb = vm.Thumbs[0];
        var currentRange = thumb.Xj - thumb.Xi;

        // update the scroll bar thumb when the user is dragging the chart
        thumb.Xi = positionInData.X - currentRange / 2;
        thumb.Xj = positionInData.X + currentRange / 2;

        // update the chart visible range
        vm.ScrollableAxes[0].MinLimit = thumb.Xi;
        vm.ScrollableAxes[0].MaxLimit = thumb.Xj;
    }

    private void OnPointerExited(object sender, PointerEventArgs e)
    {
        _isDown = false;
    }
}
