using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Axes.LabelsFormat2;

public partial class ViewModel : ObservableObject
{
    public ViewModel()
    {
        // You must register any non-Latin based font //mark
        // you can add this code when the app starts to register Chinese characters: // mark

        LiveCharts.Configure(config =>
            config.HasGlobalSKTypeface(SKFontManager.Default.MatchCharacter('汉')));

        // You can learn more about extra settings at: // mark
        // https://localhost:7053/docs/{{ platform }}/{{ version }}/Overview.Installation#configure-themes-fonts-or-mappers-optional // mark
    }

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
            LabelsPaint = new SolidColorPaint(SKColors.Black)
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
