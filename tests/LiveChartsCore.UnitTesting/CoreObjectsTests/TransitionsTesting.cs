// The MIT License(MIT)
//
// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveChartsCore.UnitTesting.CoreObjectsTests;

[TestClass]
public class TransitionsTesting
{
    [TestMethod]
    public void InfiniteAnimation()
    {
        var r = new RectangleGeometry();
        var a = (IAnimatable)r;
        var duration = TimeSpan.FromSeconds(1);
        var easing = EasingFunctions.Lineal;

        r.SetTransition(
            new Animation(easing, duration, int.MaxValue),
            nameof(r.Y), nameof(r.X), nameof(r.Width), nameof(r.Height));

        void DrawFrame(long time)
        {
            a.CurrentTime = time;
            a.IsValid = true;
        }

        float DoTransition(float from, float to, float start, float end, long time, Func<float, float> easing)
        {
            var p = (time - start) / (end - start);
            if (p > 1) p = 1;
            return from + easing(p) * (to - from);
        }

        var time = 50;
        DrawFrame(time);

        // transition from 50 to 100 for each property
        r.Y = 50;
        r.X = 50;
        r.Width = 50;
        r.Height = 50;
        r.CompleteTransition(nameof(r.Y), nameof(r.X), nameof(r.Width), nameof(r.Height));

        r.Y = 100;
        r.X = 100;
        r.Width = 100;
        r.Height = 100;

        a.CurrentTime = time;
        var startTime = 50;
        time = startTime;

        while (time <= duration.TotalMilliseconds * 2 + startTime)
        {
            DrawFrame(time);

            var x = r.X;
            var y = r.Y;
            var w = r.Width;
            var h = r.Height;
            var l = Math.Truncate((time - startTime) / duration.TotalMilliseconds);
            if ((time - startTime) % duration.TotalMilliseconds == 0 && time != startTime) l--;
            var laps = (long)(l * duration.TotalMilliseconds);
            Assert.IsTrue(x == DoTransition(50, 100, 50, 1050, time - laps, easing));
            Assert.IsTrue(y == DoTransition(50, 100, 50, 1050, time - laps, easing));
            Assert.IsTrue(w == DoTransition(50, 100, 50, 1050, time - laps, easing));
            Assert.IsTrue(h == DoTransition(50, 100, 50, 1050, time - laps, easing));

            Assert.IsTrue(!a.IsValid);
            time += 500;
        }

        // not completed yet because the duration of the animation in this case is infinite
        Assert.IsTrue(!a.IsValid);
    }

    [TestMethod]
    public void AnimationComplete()
    {
        var r = new RectangleGeometry();
        var a = (IAnimatable)r;
        var duration = TimeSpan.FromSeconds(1);
        var easing = EasingFunctions.Lineal;

        r.SetTransition(
            new Animation(easing, duration),
            nameof(r.Y), nameof(r.X), nameof(r.Width), nameof(r.Height));

        void DrawFrame(long time)
        {
            a.CurrentTime = time;
            a.IsValid = true;

            // Calling the property getter moves the transition with the current animatable time
            var x = r.X;
            var y = r.Y;
            var w = r.Width;
            var h = r.Height;
        }

        var time = 0;
        DrawFrame(time);

        // transition from 50 to 100 for each property
        r.Y = 0;
        r.X = 0;
        r.Width = 0;
        r.Height = 0;
        r.CompleteTransition(nameof(r.Y), nameof(r.X), nameof(r.Width), nameof(r.Height));
        DrawFrame(time);

        Assert.IsTrue(a.IsValid);

        r.Y = 100;
        DrawFrame(time);
        var p = r.MotionProperties[nameof(r.Y)];

        time += 500;
        DrawFrame(time);
        Assert.IsTrue(!p.IsCompleted);

        time += 500;
        DrawFrame(time);
        Assert.IsTrue(p.IsCompleted);
    }

    [TestMethod]
    public void KeyFramesTest()
    {
        var f = EasingFunctions.BuildFunctionUsingKeyFrames(
            new[]
            {
                new KeyFrame { Time = 0, Value = 0, EasingFunction = EasingFunctions.Lineal },
                new KeyFrame { Time = 0.5f, Value = 1, EasingFunction = EasingFunctions.Lineal },
                new KeyFrame { Time = 1, Value = 0, EasingFunction = EasingFunctions.Lineal },
            });

        var r = new[] { 0, 0.2f, 0.4f, 0.6f, 0.8f, 1, 0.8f, 0.6f, 0.4f, 0.2f, 0 };
        var i = 0;

        for (float t = 0; t <= 1; t += 0.1f)
        {
            var p = f(t);
            Assert.IsTrue(Math.Abs(p - r[i++]) < 0.000001);
        }

        f = EasingFunctions.BuildFunctionUsingKeyFrames(
            new[]
            {
                new KeyFrame { Time = 0, Value = 0, EasingFunction = EasingFunctions.Lineal },
                new KeyFrame { Time = 0.30f, Value = 0, EasingFunction = EasingFunctions.Lineal },
                new KeyFrame { Time = 0.80f, Value = 1, EasingFunction = EasingFunctions.Lineal },
                new KeyFrame { Time = 1, Value = 1, EasingFunction = EasingFunctions.Lineal },
            });

        r = new[] { 0, 0, 0, 0, 0.2f, 0.4f, 0.6f, 0.8f, 1, 1 };
        i = 0;

        for (float t = 0; t <= 1; t += 0.1f)
        {
            var p = f(t);
            Assert.IsTrue(Math.Abs(p - r[i++]) < 0.000001);
        }
    }

    [TestMethod]
    public void NotFinishedAnimation()
    {
        var r = new RectangleGeometry();
        var a = (IAnimatable)r;
        var duration = TimeSpan.FromMilliseconds(100);
        var easing = EasingFunctions.Lineal;

        r.SetTransition(
            new Animation(easing, duration, int.MaxValue), nameof(r.X));

        void DrawFrame(long time)
        {
            a.CurrentTime = time;
            a.IsValid = true;
        }

        var time = 0;
        DrawFrame(time);

        r.X = 0;
        r.CompleteTransition(nameof(r.X));

        r.X = 100;

        a.CurrentTime = time;
        time = 0;

        var start = 0d;
        var end = 100d;

        var sv = r.X;
        var dv = 100 - sv;

        while (time <= duration.TotalMilliseconds)
        {
            DrawFrame(time);

            if (time == 80)
            {
                sv = r.X;
                dv = 200 - sv;

                r.X = 200;
                start = time;
                end = time + 100;
            }

            var x = r.X;

            var p = (time - start) / (end - start);
            Assert.IsTrue(Math.Abs(x - (sv + p * dv)) < 0.001);

            time += 10;
        }
    }
}
