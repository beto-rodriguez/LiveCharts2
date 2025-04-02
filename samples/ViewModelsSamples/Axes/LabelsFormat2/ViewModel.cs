using SkiaSharp;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System;

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

    public double[] Values1 { get; set; } =
        [426, 583, 104];

    public double[] Values2 { get; set; } =
        [200, 558, 458];

    public string[] Labels { get; set; } =
        ["王", "赵", "张"];

    public Func<double, string> Labeler { get; set; } =
        value => value.ToString("C2");

}
