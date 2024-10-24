using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.Generic;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.SkiaSharpView.Extensions;

namespace ViewModelsSamples.Pies.Basic;

public class ViewModel
{
    // you can convert any array, list or IEnumerable<T> to a pie series collection:
    public IEnumerable<ISeries> Series { get; set; } =
        new[] { 2, 4, 1, 4, 3 }.AsPieSeries();

    // the expression above is equivalent to the next series collection:
    public IEnumerable<ISeries> Series2 { get; set; } =
        [
            new PieSeries<int> { Values = [2] },
            new PieSeries<int> { Values = [4] },
            new PieSeries<int> { Values = [1] },
            new PieSeries<int> { Values = [4] },
            new PieSeries<int> { Values = [3] },
        ];

    public LabelVisual Title { get; set; } =
        new LabelVisual
        {
            Text = "My chart title",
            TextSize = 25,
            Padding = new LiveChartsCore.Drawing.Padding(15)
        };
}
