using System;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Motion;
using LiveChartsCore.Painting;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.General.DrawOnCanvas;

public partial class ViewModel
{
    private SolidColorPaint? _paint;
    private MotionGeometry? _geometry;
    private bool _isBigCircle = true;

    [RelayCommand]
    public void ChartUpdated(ChartCommandArgs args)
    {
        var chartView = (ICartesianChartView<SkiaSharpDrawingContext>)args.Chart;

        if (_geometry is null)
        {
            _geometry = new MotionGeometry();
            _geometry.Animate(
                new Animation(EasingFunctions.BounceOut, TimeSpan.FromMilliseconds(800)));
        }

        if (_paint is null)
        {
            _paint = new SolidColorPaint(SKColors.Blue) { IsStroke = true, StrokeThickness = 2 };
            _paint.AddGeometryToPaintTask(chartView.CoreCanvas, _geometry);
            chartView.CoreCanvas.AddDrawableTask(_paint);
        }

        // lets convert the point (5, 5) in the chart values scale to pixels
        var locationInChartValues = new LvcPointD(5, 5);
        var locationInPixels = chartView.ScaleDataToPixels(locationInChartValues);

        _geometry.X = (float)locationInPixels.X;
        _geometry.Y = (float)locationInPixels.Y;
        // lets toggle the diameter of the circle between 20 and 70
        _geometry.Diameter = (_isBigCircle = !_isBigCircle) ? 70 : 20;

        // if this is the first time we draw the geometry
        // we can complete the animations.
        // if (isNewGeometry) _motionGeometry.CompleteTransition();
    }
}

public class MotionGeometry : Geometry
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

    public override void OnDraw(SkiaSharpDrawingContext context, SKPaint paint)
    {
        // we can use SkiaSharp here to draw anything we need // mark
        // https://learn.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/basics/ // mark

        // because we inherited from Geometry, this class already contains the X, Y
        // and some other motion properties that we can use.

        context.Canvas.DrawCircle(X, Y, Diameter, paint);
    }

    protected override LvcSize OnMeasure(Paint paintTasks)
    {
        // you can measure the geometry here, this method is used when the geometry
        // is used inside a layout, in this case it is not necessary.
        return new();
    }
}

