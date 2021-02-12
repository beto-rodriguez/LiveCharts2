using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharp.Drawing;
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

            var time = 0;

            r.Y = 100;
            r.X = 100;
            r.Width = 100;
            r.Height = 100;

            //r.SetStoryboard(time, new Animation(EasingFunctions.Lineal, TimeSpan.FromSeconds(1)));

            //while (time <= 1000)
            //{
            //    r.SetTime(time);

            //    //Trace.WriteLine(r.IsCompleted);

            //    time += 50;
            //}

            ////Assert.IsTrue(r.IsCompleted);

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
