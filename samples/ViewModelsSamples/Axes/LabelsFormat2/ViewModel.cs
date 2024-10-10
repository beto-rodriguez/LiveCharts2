using SkiaSharp;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Kernel.Sketches;

namespace ViewModelsSamples.Axes.LabelsFormat2;

public class ViewModel
{
    public ViewModel()
    {
        // You must register any non-Latin based font //mark
        // you can add this code when the app starts to register Chinese characters: // mark

        LiveCharts.Configure(config =>
            config.HasGlobalSKTypeface(SKFontManager.Default.MatchCharacter('汉')));

        // You can learn more about extra settings at: // mark
        // https://livecharts.dev/docs/{{ platform }}/{{ version }}/Overview.Installation#configure-themes-fonts-or-mappers-optional // mark
    }

    public ISeries[] Series { get; set; } = [
        new ColumnSeries<double> { Values = [426, 583, 104] },
        new LineSeries<double>   { Values = [200, 558, 458], Fill = null }
    ];

    public ICartesianAxis[] XAxes { get; set; } = [
        new Axis
        {
            Name = "Salesman/woman",
            Labels = ["王", "赵", "张"],
            LabelsPaint = new SolidColorPaint(SKColors.Black)
        }
    ];

    public ICartesianAxis[] YAxes { get; set; } = [
        new Axis
        {
            Name = "Sales amount",
            NamePadding = new LiveChartsCore.Drawing.Padding(0, 15),
            Labeler = Labelers.Currency,
            LabelsPaint = new SolidColorPaint
            {
                Color = SKColors.Blue,
                FontFamily = "Times New Roman",
                SKFontStyle = new SKFontStyle(
                    SKFontStyleWeight.ExtraBold,
                    SKFontStyleWidth.Normal,
                    SKFontStyleSlant.Italic)
            },
        }
    ];
}
