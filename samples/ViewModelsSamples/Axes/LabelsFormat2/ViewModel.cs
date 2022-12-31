using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Axes.LabelsFormat2;

[ObservableObject]
public partial class ViewModel
{
    public ISeries[] Series { get; set; } =
    {
        new ColumnSeries<double> { Values = new double[] { 426, 583, 104 } },
        new LineSeries<double>   { Values = new double[] { 200, 558, 458 }, Fill = null }
    };

    public Axis[] XAxes { get; set; } =
    {
        new Axis
        {
            Name = "Salesman/woman",
            Labels = new string[] { "王", "赵", "张" },
            LabelsPaint = new SolidColorPaint
            {
                Color = SKColors.Black,

                // you need to enable the Chinese characters for SkiaSharp
                // use the SKFontManager.Default.MatchCharacter() SkiaSharp function.

                SKTypeface = SKFontManager.Default.MatchCharacter('汉') // 汉语 // mark
                // SKTypeface = SKFontManager.Default.MatchCharacter('أ'), // Arab
                // SKTypeface = SKFontManager.Default.MatchCharacter('あ'), // Japanese
                // SKTypeface = SKFontManager.Default.MatchCharacter('Ж'), // Russian

                // You can also register a default global SKTypeface // mark
                // this will load the font in any Paint when the SKTypeface property is null. // mark
                // for more info see: ToDo: Add link!!! // mark
            }
        }
    };

    public Axis[] YAxes { get; set; } =
    {
        new Axis
        {
            Name = "Sales amount",
            NamePadding = new LiveChartsCore.Drawing.Padding(0, 15),
            Labeler = Labelers.Currency,
            LabelsPaint = new SolidColorPaint
            {
                Color = SKColors.Blue,
                FontFamily = "Times New Roman",
                SKFontStyle = new SKFontStyle(SKFontStyleWeight.ExtraBold, SKFontStyleWidth.Normal, SKFontStyleSlant.Italic)
            },
        }
    };
}
