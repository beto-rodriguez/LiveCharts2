using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Avalonia;
using LiveChartsCore.SkiaSharpView.Drawing;
using ViewModelsSamples.General.Scrollable;

namespace AvaloniaSample.General.Scrollable;

public class View : UserControl
{
    private bool _isDown = false;

    public View()
    {
        InitializeComponent();
    }

    private void OnChart_Updated(IChartView<SkiaSharpDrawingContext> chart)
    {
        var vm = (ViewModel)DataContext!;
        var cartesianChart = (CartesianChart)chart;

        var x = cartesianChart.XAxes.First();

        // update the scroll bar thumb when the chart is updated (zoom/pan)
        // this will let the user know the current visible range
        var thumb = vm.Thumbs[0];

        thumb.Xi = x.MinLimit;
        thumb.Xj = x.MaxLimit;
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _isDown = true;
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isDown) return;

        var vm = (ViewModel)DataContext!;
        var scrollBarChart = this.Find<CartesianChart>("ScrollBarChart");

        var pointerPosition = e.GetPosition(scrollBarChart);
        var positionInData = scrollBarChart.ScalePixelsToData(new(pointerPosition.X, pointerPosition.Y));

        var thumb = vm.Thumbs[0];
        var currentRange = thumb.Xj - thumb.Xi;

        // update the scroll bar thumb when the user is dragging the chart
        thumb.Xi = positionInData.X - currentRange / 2;
        thumb.Xj = positionInData.X + currentRange / 2;

        // update the chart visible range
        vm.ScrollableAxes[0].MinLimit = thumb.Xi;
        vm.ScrollableAxes[0].MaxLimit = thumb.Xj;
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _isDown = false;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
