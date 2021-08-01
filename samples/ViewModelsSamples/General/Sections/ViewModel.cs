using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace ViewModelsSamples.General.Sections
{
    public class ViewModel
    {
        public ObservableCollection<RectangularSection> Sections { get; set; }
            = new ObservableCollection<RectangularSection>
            {
                new RectangularSection
                {
                    // creates a section from 3 to 4 in the X axis
                    Xi = 3,
                    Xj = 4,
                    Fill = new SolidColorPaintTask(new SKColor(255, 205, 210))
                },

                new RectangularSection
                {
                    // creates a section from 5 to 6 in the X axis
                    // and from 2 to 8 in the Y axis
                    Xi = 5,
                    Xj = 6,
                    Yi = 2,
                    Yj = 8,
                    Fill = new SolidColorPaintTask(new SKColor(187, 222, 251))
                },

                 new RectangularSection
                {
                    // creates a section from 8 to the end in the X axis
                    Xi = 8,
                    Fill = new SolidColorPaintTask(new SKColor(249, 251, 231))
                }
            };

        public ObservableCollection<ISeries> Series { get; set; }
            = new ObservableCollection<ISeries>
        {
            new ScatterSeries<ObservablePoint>
            {
                Values = new ObservableCollection<ObservablePoint>
                {
                    new ObservablePoint(2.2, 5.4),
                    new ObservablePoint(4.5, 2.5),
                    new ObservablePoint(4.2, 7.4),
                    new ObservablePoint(6.4, 9.9),
                    new ObservablePoint(4.2, 9.2),
                    new ObservablePoint(5.8, 3.5),
                    new ObservablePoint(7.3, 5.8),
                    new ObservablePoint(8.9, 3.9),
                    new ObservablePoint(6.1, 4.6),
                    new ObservablePoint(9.4, 7.7),
                    new ObservablePoint(8.4, 8.5),
                    new ObservablePoint(3.6, 9.6),
                    new ObservablePoint(4.4, 6.3),
                    new ObservablePoint(5.8, 4.8),
                    new ObservablePoint(6.9, 3.4),
                    new ObservablePoint(7.6, 1.8),
                    new ObservablePoint(8.3, 8.3),
                    new ObservablePoint(9.9, 5.2),
                    new ObservablePoint(8.1, 4.7),
                    new ObservablePoint(7.4, 3.9),
                    new ObservablePoint(6.8, 2.3),
                    new ObservablePoint(5.3, 7.1),
                }
            }
        };
    }
}
