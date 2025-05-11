using System;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Generators;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.General.DrawOnCanvas;

// in this example, we use the UpdateStartedCommand to add our custom // mark
// geometry to the chart, then we use the PointerPressedCommand to change the //mark
// size of the geometry when the user clicks on it. // mark

public partial class ViewModel
{
    private bool _isBig;
    private readonly MotionGeometry _geometry = new()
    {
        Diameter = 20,
        Fill = new SolidColorPaint(SKColors.Blue.WithAlpha(100)),
        Stroke = new SolidColorPaint(SKColors.Blue, strokeWidth: 3)
    };

    [RelayCommand]
    public void ChartUpdated(ChartCommandArgs args)
    {
        var chartView = (ICartesianChartView)args.Chart;

        // lets place the geometry at (5, 5) in the chart values scale
        var locationInChartValues = new LvcPointD(5, 5);
        var locationInPixels = chartView.ScaleDataToPixels(locationInChartValues);

        _geometry.X = (float)locationInPixels.X;
        _geometry.Y = (float)locationInPixels.Y;

        _geometry.Initialize(args);
    }

    [RelayCommand]
    public void ChartPressed(PointerCommandArgs args)
    {
        _geometry.Diameter = _isBig ? 20 : 70;
        _isBig = !_isBig;
        args.Chart.Invalidate();
    }
}

public partial class MotionGeometry : BoundedDrawnGeometry, IDrawnElement<SkiaSharpDrawingContext>
{
    private bool _isInitialized;

    // The MotionProperty attribute makes the property animatable, // mark
    // both the property and the class must be marked as partial // mark
    // partial properties were introduced in C# 13 // mark
    // A motion property will animate any change in the property // mark
    // according to the Animation defined in the Animate method. // mark

    [MotionProperty]
    public partial float Diameter { get; set; }

    public void Draw(SkiaSharpDrawingContext context)
    {
        // we can use SkiaSharp here to draw anything we need // mark
        // https://learn.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/basics/ // mark

        // because we inherited from BoundedDrawnGeometry, this class already contains the X, Y
        // and some other motion properties that we can use.

        var paint = context.ActiveSkiaPaint;
        context.Canvas.DrawCircle(X, Y, Diameter, paint);
    }

    public void Initialize(ChartCommandArgs args)
    {
        if (_isInitialized) return;

        args.Chart.CoreChart.Canvas.AddGeometry(this);

        var animation = new Animation(
            easingFunction: EasingFunctions.BounceOut,
            duration: TimeSpan.FromMilliseconds(800));

        this.Animate(animation);

        _isInitialized = true;
    }
}

