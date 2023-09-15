using LiveChartsCore;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using LiveChartsCore.SkiaSharpView.Extensions;

namespace ViewModelsSamples.Pies.Doughnut;

public partial class ViewModel : ObservableObject
{
    // you can convert any array, list or IEnumerable<T> to a pie series collection:
    public IEnumerable<ISeries> Series { get; set; } =
        new[] { 2, 4, 1, 4, 3 }.AsPieSeries((value, series) =>
        {
            series.InnerRadius = 70;
        });
}
