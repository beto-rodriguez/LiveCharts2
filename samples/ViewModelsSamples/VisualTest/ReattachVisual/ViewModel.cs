using System.Collections.Generic;
using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.VisualTest.ReattachVisual;

public class ViewModel
{
    public IEnumerable<ISeries> Series { get; set; }
        = new ObservableCollection<ISeries>
        {
                new ColumnSeries<double>
                {
                    Values = new ObservableCollection<double> { 2, 5, 4, -2, 4, -3, 5 }
                }
        };

    public IEnumerable<ISeries> PieSeries { get; set; }
        = new ObservableCollection<ISeries>
        {
                new PieSeries<double> { Values = new ObservableCollection<double> { 5 } },
                new PieSeries<double> { Values = new ObservableCollection<double> { 4 } },
                new PieSeries<double> { Values = new ObservableCollection<double> { 3 } }
        };

    public IEnumerable<ISeries> PolarSeries { get; set; }
        = new ObservableCollection<ISeries>
        {
               new PolarLineSeries<double>
               {
                   Values = new ObservableCollection<double> { 2, 5, 4, -2, 4, -3, 5 }
               }
        };

    public IEnumerable<RectangularSection> Sections { get; set; }
        = new ObservableCollection<RectangularSection>
        {
                new RectangularSection
                {
                    Xi = 0,
                    Xj = 4,
                    Fill = new SolidColorPaint(SKColors.LightGray)
                }
        };
}
