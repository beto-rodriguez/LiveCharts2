using LiveChartsCore.Kernel;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace LiveChartsCore.SkiaSharpView.Xamarin.Forms
{
    public class TooltipBindingContext
    {
        public IEnumerable<TooltipPoint>? Points { get; set; } = Enumerable.Empty<TooltipPoint>();

        public string? FontFamily { get; set; }

        public double FontSize { get; set; }

        public Color TextColor { get; set; }

        public FontAttributes FontAttributes { get; set; }
    }
}