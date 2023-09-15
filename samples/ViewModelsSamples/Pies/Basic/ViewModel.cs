using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.SkiaSharpView.Extensions;

namespace ViewModelsSamples.Pies.Basic;

public partial class ViewModel : ObservableObject
{
    // you can convert any array, list or IEnumerable<T> to a pie series collection:
    public IEnumerable<ISeries> Series { get; set; } =
        new[] { 2, 4, 1, 4, 3 }.AsPieSeries();

    // the expression above is equivalent to the next series collection:
    public IEnumerable<ISeries> Series2 { get; set; } =
        new[]
        {
            new PieSeries<int> { Values = new[]{ 2 } },
            new PieSeries<int> { Values = new[]{ 4 } },
            new PieSeries<int> { Values = new[]{ 1 } },
            new PieSeries<int> { Values = new[]{ 4 } },
            new PieSeries<int> { Values = new[]{ 3 } },
        };

    public LabelVisual Title { get; set; } =
        new LabelVisual
        {
            Text = "My chart title",
            TextSize = 25,
            Padding = new LiveChartsCore.Drawing.Padding(15),
            Paint = new SolidColorPaint(SKColors.DarkSlateGray)
        };
}
