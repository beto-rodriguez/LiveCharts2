using SkiaSharp;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;

namespace ViewModelsSamples.General.Sections2;

public class ViewModel
{
    public RectangularSection[] Sections { get; set; } = [
        new RectangularSection
        {
            Yi = 8,
            Yj = 8,
            Stroke = new SolidColorPaint
            {
                Color = SKColors.Red,
                StrokeThickness = 3,
                PathEffect = new DashEffect([6, 6])
            }
        },
        new RectangularSection
        {
            Xi = 4,
            Xj = 6,
            Fill = new SolidColorPaint { Color = SKColors.Blue.WithAlpha(20) }
        },
    ];

    public ISeries[] Series { get; set; } = [
        new ScatterSeries<ObservablePoint>
        {
            GeometrySize = 10,
            Stroke = new SolidColorPaint { Color = SKColors.Blue, StrokeThickness = 1 },
            Fill = null,
            Values =
            [
                new(2.2, 5.4), new(4.5, 2.5), new(4.2, 7.4),
                new(6.4, 9.9), new(4.2, 9.2), new(5.8, 3.5),
                new(7.3, 5.8), new(8.9, 3.9), new(6.1, 4.6),
                new(9.4, 7.7), new(8.4, 8.5), new(3.6, 9.6),
                new(4.4, 6.3), new(5.8, 4.8), new(6.9, 3.4),
                new(7.6, 1.8), new(8.3, 8.3), new(9.9, 5.2),
                new(8.1, 4.7), new(7.4, 3.9), new(6.8, 2.3)
            ]
        }
    ];
}
