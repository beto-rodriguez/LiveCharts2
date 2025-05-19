using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace MauiSample.Bars.DelayedAnimation;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class View : ContentPage
{
    public View()
    {
        InitializeComponent();
        //series.PointMeasured += XamlColumnSeries_PointMeasured;
    }


    private void XamlColumnSeries_PointMeasured(ChartPoint<object, RoundedRectangleGeometry, LabelGeometry> obj)
    {
        var a = obj;
    }

    private void a(ChartPoint obj)
    {
        var a = obj;
    }
}
