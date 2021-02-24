using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

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

            float x = 0, y = 0, w = 0, h = 0;

            var startTime = 50;
            time = startTime;

            while (time <= duration.TotalMilliseconds * 2 + startTime)
            {
                DrawFrame(time);

                x = r.X;
                y = r.Y;
                w = r.Width;
                h = r.Height;

                var l = Math.Truncate((time - startTime) / duration.TotalMilliseconds);
                if ((time - startTime) % duration.TotalMilliseconds == 0 && time != startTime) l = l - 1;
                var laps = (long)(l * duration.TotalMilliseconds);
                Assert.IsTrue(x == DoTransition(50, 100, 50, 1050, time - laps, easing));
                Assert.IsTrue(y == DoTransition(50, 100, 50, 1050, time - laps, easing));
                Assert.IsTrue(w == DoTransition(50, 100, 50, 1050, time - laps, easing));
                Assert.IsTrue(h == DoTransition(50, 100, 50, 1050, time - laps, easing));

                Assert.IsTrue(!a.IsCompleted);
                time += 500;
            }

            //DrawFrame(1000);
            //Assert.IsTrue(!a.IsCompleted);
            //time = 1000;
            //r.Height = 200;

            //r.SetStoryboard(time, new Animation(EasingFunctions.Lineal, TimeSpan.FromSeconds(1)));
            ////Assert.IsTrue(!r.IsCompleted);

            //while (time <= 2000)
            //{
            //    r.SetTime(time);

            //    //Trace.WriteLine(r.IsCompleted);

            //    time += 50;
            //}

            ////Assert.IsTrue(r.IsCompleted);
        }
    }
}
