using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Error.Basic;

public partial class ViewModel : ObservableObject
{
    public ISeries[] Series { get; set; } =
    {
        // use the ErrorValue type to define the error in Y
        new ColumnSeries<ErrorValue>
        {
            Values = new ErrorValue[]
            {
                // (Y, Y+- error) // mark
                new(65, 6),
                // (Y, Y+ error, Y- error) // mark
                new(70, 15, 4),
                new(35, 4),
                new(70, 6),
                new(30, 5),
                new(60, 4, 16),
                new(65, 6)
            },
            ErrorPaint = new SolidColorPaint(SKColors.Black)
        },

        // When you need to define the error in X and Y use the ErrorPoint type // mark
        new ColumnSeries<ErrorPoint>
        {
            Values = new ErrorPoint[]
            {
                // (X, Y, Y+- error, Y+- error) // mark
                new(0, 50, 0.2, 8),
                // (X, Y, X- error, X+ erorr, Y+ error, Y- error) // mark
                new(1, 45, 0.1, 0.3, 15, 4),
                new(2, 25, 0.3, 4),
                new(3, 30, 0.2, 6),
                new(4, 70, 0.2, 8),
                new(5, 30, 0.4, 4),
                new(6, 50, 0.3, 6)
            },
            ErrorPaint = new SolidColorPaint(SKColors.Black)
        }

        // Error marks are also supported in LineSeries<T> and RowSeries<T> // mark
    };
}
