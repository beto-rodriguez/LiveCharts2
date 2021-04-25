using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ViewModelsSamples.Axes.Multiple
{
    public class ViewModel
    {
        public ViewModel()
        {
            var blue = new SKColor(25, 118, 210);
            var red = new SKColor(229, 57, 53);
            var yellow = new SKColor(198, 167, 0);

            Series = new ObservableCollection<ISeries>
            {
                new LineSeries<double>
                {
                    LineSmoothness = 1,
                    Name = "tens",
                    Values = new ObservableCollection<double> { 14, 13, 14, 15, 17 },
                    Stroke = new SkiaSharpPaintTask(blue, 2),
                    GeometryStroke = new SkiaSharpPaintTask(blue, 2),
                    Fill = null,
                    ScalesYAt = 0 // it will be scaled at the Axis[0] instance
                },
                new LineSeries<double>
                {
                    Name = "tens 2",
                    Values = new ObservableCollection<double> { 11, 12, 13, 10, 13 },
                    Stroke = new SkiaSharpPaintTask(blue, 2),
                    GeometryStroke = new SkiaSharpPaintTask(blue, 2),
                    Fill = null,
                    ScalesYAt = 0 // it will be scaled at the Axis[0] instance
                },
                new LineSeries<double>
                {
                    Name = "hundreds",
                    Values = new ObservableCollection<double> { 533, 586, 425, 579, 518 },
                    Stroke = new SkiaSharpPaintTask(red, 2),
                    GeometryStroke = new SkiaSharpPaintTask(red, 2),
                    Fill = null,
                    ScalesYAt = 1 // it will be scaled at the YAxes[1] instance
                },
                new LineSeries<double>
                {
                    Name = "thousands",
                    Values = new ObservableCollection<double> { 5493, 7843, 4368, 9018, 3902 },
                    Stroke = new SkiaSharpPaintTask(yellow, 2),
                    GeometryStroke = new SkiaSharpPaintTask(yellow, 2),
                    Fill = null,
                    ScalesYAt = 2  // it will be scaled at the YAxes[2] instance
                }
            };

            YAxes = new List<Axis>
            {
                new Axis // the "units" and "tens" series will be scaled on this axis
                {
                    TextBrush = new SkiaSharpPaintTask(blue)
                },
                new Axis // the "hundreds" series will be scaled on this axis
                {
                    TextBrush = new SkiaSharpPaintTask(red),
                    ShowSeparatorLines = false,
                    Position = LiveChartsCore.Measure.AxisPosition.End
                },
                new Axis() // the "thousands" series will be scaled on this axis
                {
                    TextBrush = new SkiaSharpPaintTask(yellow),
                    ShowSeparatorLines = false,
                    Position = LiveChartsCore.Measure.AxisPosition.End
                }
            };
        }

        public IEnumerable<ISeries> Series { get; set; }

        public IEnumerable<IAxis> YAxes { get; set; }
    }
}
