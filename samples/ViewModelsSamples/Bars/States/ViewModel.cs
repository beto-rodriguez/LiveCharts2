using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;

namespace ViewModelsSamples.Bars.States
{
    public class ViewModel
    {
        public ViewModel()
        {
            var columnSeries = new ColumnSeries<int>
            {
                Values = new List<int>
                {
                    24,
                    12,
                    35, // hot!
                    10,
                    15,
                    28,
                    32  // hot!
                },
                Stroke = null,
                Fill = new SolidColorPaintTask(SKColors.CornflowerBlue)
            };

            var isStateDefined = false;

            // use the point measured event to define the "hotDay" state
            columnSeries.PointMeasured += (point) =>
            {
                if (!isStateDefined)
                {
                    var skiaChart = (IChartView<SkiaSharpDrawingContext>)point.Context.Chart;
                    skiaChart.PointStates["hotDay"] =
                        new StrokeAndFillDrawable<SkiaSharpDrawingContext>(null, new SolidColorPaintTask(SKColors.OrangeRed));
                    isStateDefined = true;
                }

                if (point.PrimaryValue > 30) point.AddToState("hotDay");
                else point.RemoveFromState("hotDay");
            };

            Series = new List<ISeries> { columnSeries };
        }

        public List<ISeries> Series { get; set; }
    }
}
