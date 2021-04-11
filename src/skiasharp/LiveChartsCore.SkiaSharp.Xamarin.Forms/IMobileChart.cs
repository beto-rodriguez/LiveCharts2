using Xamarin.Forms;

namespace LiveChartsCore.SkiaSharpView.Xamarin.Forms
{
    public interface IMobileChart
    {
        DataTemplate TooltipTemplate { get; set; }

        string TooltipFontFamily { get; set; }

        double TooltipFontSize { get; set; }

        Color TooltipTextColor { get; set; }

        Color TooltipBackgroundColor { get; set; }

        FontAttributes TooltipFontAttributes { get; set; }

        DataTemplate LegendTemplate { get; set; }

        string LegendFontFamily { get; set; }

        double LegendFontSize { get; set; }

        Color LegendTextColor { get; set; }

        Color LegendBackgroundColor { get; set; }

        FontAttributes LegendFontAttributes { get; set; }
    }
}