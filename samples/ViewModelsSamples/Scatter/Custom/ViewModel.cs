using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ViewModelsSamples.Scatter.Custom
{
    public class ViewModel
    {
        public ViewModel()
        {
            var r = new Random();
            var values1 = new ObservableCollection<ObservablePoint>();
            var values2 = new ObservableCollection<ObservablePoint>();

            for (var i = 0; i < 20; i++)
            {
                values1.Add(new ObservablePoint(r.Next(0, 20), r.Next(0, 20)));
                values2.Add(new ObservablePoint(r.Next(0, 20), r.Next(0, 20)));
            }

            Series = new ObservableCollection<ISeries>
            {
                // use the second type argument to specify the geometry to draw for every point
                // there are already many predefined geometries in the
                // LiveChartsCore.SkiaSharpView.Drawing.Geometries namespace
                new ScatterSeries<ObservablePoint, RoundedRectangleGeometry>
                {
                    Values = values1,
                    Stroke = null,
                    GeometrySize = 40,
                },

                // Or Define your own SVG geometry
                new ScatterSeries<ObservablePoint, MyGeometry>
                {
                    Values = values2,
                    GeometrySize = 40
                }
            };
        }

        public IEnumerable<ISeries> Series { get; set; }
    }

    public class MyGeometry : SVGPathGeometry
    {
        // Icon made by Pixel perfect from www.flaticon.com
        // https://www.flaticon.com/free-icon/target_726157?term=target&page=1&position=72&page=1&position=72&related_id=726157&origin=search
        public static SKPath svgPath = SKPath.ParseSvgPathData(
            "M510.795,57.902c-2.464-5.984-8.32-9.888-14.784-9.888h-32v-32c0-6.464-3.904-12.32-9.888-14.784    " +
            "c-5.952-2.496-12.832-1.12-17.44,3.456l-48,48c-2.976,3.008-4.672,7.072-4.672,11.328v41.376L212.683,276.686    " +
            "c-6.24,6.24-6.24,16.384,0,22.624c3.136,3.136,7.232,4.704,11.328,4.704s8.192-1.568,11.328-4.672l171.296-171.328h41.376    " +
            "c4.256,0,8.32-1.696,11.328-4.672l48-48C511.883,70.734,513.259,63.886,510.795,57.902z" +
            "M342.027,238.574l-54.56,54.752c-2.72,32.832-29.92,58.688-63.456,58.688c-35.36,0-64-28.64-64-64    " +
            "c0-34.208,26.912-61.92,60.672-63.68l53.728-53.92c-15.488-6.656-32.48-10.4-50.4-10.4c-70.592,0-128,57.408-128,128    " +
            "s57.408,128,128,128s128-57.408,128-128C352.011,270.478,348.427,253.806,342.027,238.574z" +
            "M412.715,167.662l-47.168,47.328c11.392,21.984,18.464,46.592,18.464,73.024c0,88.224-71.776,160-160,160    " +
            "s-160-71.776-160-160s71.776-160,160-160c26.784,0,51.648,7.232,73.856,18.912l47.104-47.264    " +
            "c-34.912-22.464-76.384-35.648-120.96-35.648c-123.712,0-224,100.288-224,224s100.288,224,224,224s224-100.288,224-224    " +
            "C448.011,243.662,434.955,202.446,412.715,167.662z");

        public MyGeometry()
            : base(svgPath)
        {

        }
    }
}
