using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.VisualElements;

namespace ViewModelsSamples.Lines.Basic;

public class ViewModel
{
    public ISeries[] Series { get; set; } = [
        new LineSeries<double>
        {
            Values = [2, 1, 3, 5, 3, 4, 6],
            Fill = null,
            GeometrySize = 20
        },
        new LineSeries<int, StarGeometry>
        {
            Values = [4, 2, 5, 2, 4, 5, 3],
            Fill = null,
            GeometrySize = 20,
        }
    ];

    public LabelVisual Title { get; set; } =
        new LabelVisual
        {
            Text = "My chart title",
            TextSize = 25,
            Padding = new LiveChartsCore.Drawing.Padding(15)
        };
}
