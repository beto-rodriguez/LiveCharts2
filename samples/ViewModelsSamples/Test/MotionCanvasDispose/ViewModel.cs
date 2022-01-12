using System;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Test.MotionCanvasDispose;

public class ViewModel
{
    public static void Generate(MotionCanvas<SkiaSharpDrawingContext> canvas)
    {
        var r = new Random();
        var p = new SolidColorPaint(SKColors.Blue, 3) { IsFill = true };

        canvas.AddDrawableTask(p);

        for (var i = 0; i < 1000; i++)
        {
            var circle = new CircleGeometry { X = r.Next(15, 285), Y = r.Next(15, 285), Width = 5, Height = 5 };

            _ = circle
                .TransitionateProperties(
                    nameof(circle.X), nameof(circle.Y))
                .WithAnimation(animation =>
                    animation
                        .WithDuration(TimeSpan.FromSeconds(1))
                        .WithEasingFunction(EasingFunctions.ElasticOut))
                .CompleteCurrentTransitions();

            //circle.SetPropertiesTransitions(
            //    new Animation(EasingFunctions.ElasticOut, TimeSpan.FromSeconds(1)),
            //    nameof(circle.X), nameof(circle.Y));
            //circle.CompleteAllTransitions();

            p.AddGeometryToPaintTask(canvas, circle);

            circle.X = r.Next(15, 285);
            circle.Y = r.Next(15, 285);
        }

        canvas.Invalidate();
    }
}
