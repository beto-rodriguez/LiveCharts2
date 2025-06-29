using System;
using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Generators;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Eto;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace EtoFormsSample.General.DrawOnCanvas;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;
    private readonly MotionGeometry _geometry;
    private bool _isBig;

    public View()
    {
        cartesianChart = new CartesianChart();

        _geometry = new MotionGeometry
        {
            Diameter = 20,
            Fill = new SolidColorPaint(SKColors.Blue.WithAlpha(100)),
            Stroke = new SolidColorPaint(SKColors.Blue, strokeWidth: 3)
        };

        cartesianChart.UpdateStarted += chart =>
        {
            var locationInChartValues = new LvcPointD(5, 5);
            var locationInPixels = cartesianChart.ScaleDataToPixels(locationInChartValues);
            _geometry.X = (float)locationInPixels.X;
            _geometry.Y = (float)locationInPixels.Y;
            _geometry.Initialize(chart);
        };

        cartesianChart.MouseDown += (s, e) =>
        {
            _geometry.Diameter = _isBig ? 20 : 70;
            _isBig = !_isBig;
            cartesianChart.Invalidate();
        };

        Content = cartesianChart;
    }
}

public partial class MotionGeometry : BoundedDrawnGeometry, IDrawnElement<SkiaSharpDrawingContext>
{
    private bool _isInitialized;

    [MotionProperty]
    public partial float Diameter { get; set; }

    public void Draw(SkiaSharpDrawingContext context)
    {
        var paint = context.ActiveSkiaPaint;
        context.Canvas.DrawCircle(X, Y, Diameter, paint);
    }

    public void Initialize(IChartView chart)
    {
        if (_isInitialized) return;
        chart.CoreChart.Canvas.AddGeometry(this);

        var animation = new Animation(
            easingFunction: EasingFunctions.BounceOut,
            duration: TimeSpan.FromMilliseconds(800));

        this.Animate(animation);

        _isInitialized = true;
    }
}
