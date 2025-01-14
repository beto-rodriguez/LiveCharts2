using System;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.General.DrawOnCanvas;

public partial class ViewModel
{
    private readonly MotionGeometry _geometry = new() { Stroke = new SolidColorPaint(SKColors.Blue) };
    private bool _isInitialized;
    private bool _isBigCircle = true;

    [RelayCommand]
    public void ChartUpdated(ChartCommandArgs args)
    {
        var chartView = (ICartesianChartView)args.Chart;

        // lets convert the point (5, 5) in the chart values scale to pixels
        var locationInChartValues = new LvcPointD(5, 5);
        var locationInPixels = chartView.ScaleDataToPixels(locationInChartValues);

        _geometry.X = (float)locationInPixels.X;
        _geometry.Y = (float)locationInPixels.Y;
        // lets toggle the diameter of the circle between 20 and 70
        _geometry.Diameter = (_isBigCircle = !_isBigCircle) ? 70 : 20;

        if (!_isInitialized)
        {
            chartView.Core.Canvas.AddGeometry(_geometry);
            _geometry.Animate(
                new Animation(EasingFunctions.BounceOut, TimeSpan.FromMilliseconds(800)));
            _isInitialized = true;
        }
    }
}

public class MotionGeometry : BoundedDrawnGeometry, IDrawnElement<SkiaSharpDrawingContext>
{
    // use Motion properties to animate the geometry
    private readonly FloatMotionProperty _diameter;

    public MotionGeometry()
    {
        _diameter = RegisterMotionProperty(new FloatMotionProperty(nameof(Diameter), 0));
    }

    public float Diameter
    {
        get => _diameter.GetMovement(this);
        set => _diameter.SetMovement(value, this);
    }

    public void Draw(SkiaSharpDrawingContext context)
    {
        // we can use SkiaSharp here to draw anything we need // mark
        // https://learn.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/basics/ // mark

        // because we inherited from Geometry, this class already contains the X, Y
        // and some other motion properties that we can use.

        var paint = context.ActiveSkiaPaint;
        context.Canvas.DrawCircle(X, Y, Diameter, paint);
    }
}

