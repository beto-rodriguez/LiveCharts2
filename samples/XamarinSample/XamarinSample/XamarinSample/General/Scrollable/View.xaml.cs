using System.Linq;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Xamarin.Forms;
using ViewModelsSamples.General.Scrollable;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinSample.General.Scrollable;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class View : ContentPage
{
    private bool _isDown = false;

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

    private void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
    {
        // This event is not fired when the user is dragging the chart
        // ToDo: check why.

        switch (e.StatusType)
        {
            case GestureStatus.Started:
                _isDown = true;
                break;
            case GestureStatus.Running:
                var x = e.TotalX * DeviceDisplay.MainDisplayInfo.Density;
                var y = e.TotalY * DeviceDisplay.MainDisplayInfo.Density;
                DragThumb(x, y);
                break;
            case GestureStatus.Completed:
            case GestureStatus.Canceled:
            default:
                _isDown = false;
                break;
        }
    }

    private void DragThumb(double x, double y)
    {
        var vm = (ViewModel)BindingContext;
        var scrollBarChart = ScrollBarChart;

        var positionInData = scrollBarChart.ScalePixelsToData(new(x, y));

        var thumb = vm.Thumbs[0];
        var currentRange = thumb.Xj - thumb.Xi;

        // update the scroll bar thumb when the user is dragging the chart
        thumb.Xi = positionInData.X - currentRange / 2;
        thumb.Xj = positionInData.X + currentRange / 2;

        // update the chart visible range
        vm.ScrollableAxes[0].MinLimit = thumb.Xi;
        vm.ScrollableAxes[0].MaxLimit = thumb.Xj;
    }
}
