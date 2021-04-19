using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LiveChartsCore.UnitTesting
{
    [TestClass]
    public class TransitionsTesting
    {
        [TestMethod]
        public void TestMethod1()
        {
            var r = new RectangleGeometry();
            var a = (IAnimatable)r;
            var duration = TimeSpan.FromSeconds(1);
            var easing = EasingFunctions.Lineal;

            r.SetPropertiesTransitions(
                new Animation(easing, duration, int.MaxValue),
                nameof(r.Y), nameof(r.X), nameof(r.Width), nameof(r.Height));

            void DrawFrame(long time)
            {
                a.CurrentTime = time;
                a.IsCompleted = true;
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
            r.CompleteTransitions(nameof(r.Y), nameof(r.X), nameof(r.Width), nameof(r.Height));

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

                Assert.IsTrue(!a.IsCompleted);
                time += 500;
            }

            // not completed yet because the duration of the animation in this case is infinite
            Assert.IsTrue(!a.IsCompleted);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var r = new RectangleGeometry();
            var a = (IAnimatable)r;
            var duration = TimeSpan.FromSeconds(1);
            var easing = EasingFunctions.Lineal;

            r.SetPropertiesTransitions(
                new Animation(easing, duration),
                nameof(r.Y), nameof(r.X), nameof(r.Width), nameof(r.Height));

            void DrawFrame(long time)
            {
                a.CurrentTime = time;
                a.IsCompleted = true;

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
            r.CompleteTransitions(nameof(r.Y), nameof(r.X), nameof(r.Width), nameof(r.Height));
            DrawFrame(time);

            Assert.IsTrue(a.IsCompleted);

            r.Y = 100;
            DrawFrame(time);
            var p = r.GetTransitionProperty(nameof(r.Y));

            time += 500;
            DrawFrame(time);
            Assert.IsTrue(!p.IsCompleted);

            time += 500;
            DrawFrame(time);
            Assert.IsTrue(p.IsCompleted);
        }
    }
}
