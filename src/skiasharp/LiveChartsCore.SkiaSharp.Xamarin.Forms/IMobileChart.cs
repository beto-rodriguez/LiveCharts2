using Xamarin.Forms;

namespace LiveChartsCore.SkiaSharpView.Xamarin.Forms
{
    public interface IMobileChart
    {
        DataTemplate TooltipTemplate { get; set; }

        string TooltipFontFamily { get; set; }

        double TooltipFontSize { get; set; }

        Color TooltipTextColor { get; set; }

        FontAttributes TooltipFontAttributes { get; set; }
    }
}