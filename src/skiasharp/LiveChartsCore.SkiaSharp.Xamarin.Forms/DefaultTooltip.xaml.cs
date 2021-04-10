using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView.Drawing;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LiveChartsCore.SkiaSharpView.Xamarin.Forms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DefaultTooltip : ContentView, IChartTooltip<SkiaSharpDrawingContext>
    {
        private readonly DataTemplate defaultTemplate;
        private readonly Dictionary<ChartPoint, object> activePoints = new Dictionary<ChartPoint, object>();

        public DefaultTooltip()
        {
            InitializeComponent();
            BindingContext = this;
            defaultTemplate = Resources["defaultTemplate"] as DataTemplate;
        }

        public DataTemplate TooltipTemplate { get; set; }

        public IEnumerable<TooltipPoint> Points { get; set; }

        public string FontFamily { get; set; }

        public double FontSize { get; set; }

        public Color TextColor { get; set; }

        public FontAttributes FontAttributes { get; set; }

        public void Show(IEnumerable<TooltipPoint> tooltipPoints, Chart<SkiaSharpDrawingContext> chart)
        {
            var mobileChart = (IMobileChart)chart.View;

            if (!tooltipPoints.Any())
            {
                foreach (var key in activePoints.Keys.ToArray())
                {
                    key.RemoveFromHoverState();
                    activePoints.Remove(key);
                }
                return;
            }

            Points = tooltipPoints;

            System.Drawing.PointF? location = null;
            var size = new Size
            {
                Width = Width * DeviceDisplay.MainDisplayInfo.Density,
                Height = Height * DeviceDisplay.MainDisplayInfo.Density
            };

            if (chart is CartesianChart<SkiaSharpDrawingContext>)
            {
                location = tooltipPoints.GetCartesianTooltipLocation(
                    chart.TooltipPosition, new System.Drawing.SizeF((float)size.Width, (float)size.Height));
            }
            if (chart is PieChart<SkiaSharpDrawingContext>)
            {
                location = tooltipPoints.GetPieTooltipLocation(
                    chart.TooltipPosition, new System.Drawing.SizeF((float)size.Width, (float)size.Height));
            }

            var template = mobileChart.TooltipTemplate ?? defaultTemplate;
            if (TooltipTemplate != template) TooltipTemplate = template;
            FontFamily = mobileChart.TooltipFontFamily;
            TextColor = mobileChart.TooltipTextColor;
            FontSize = mobileChart.TooltipFontSize;
            FontAttributes = mobileChart.TooltipFontAttributes;
            BuildContent();

            Measure(double.PositiveInfinity, double.PositiveInfinity);
            var chartSize = chart.ControlSize;

            AbsoluteLayout.SetLayoutBounds(
                this,
                new Rectangle(
                    location.Value.X / chartSize.Width, location.Value.Y / chartSize.Height,
                    AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

            var o = new object();
            foreach (var tooltipPoint in tooltipPoints)
            {
                tooltipPoint.Point.AddToHoverState();
                activePoints[tooltipPoint.Point] = o;
            }

            foreach (var key in activePoints.Keys.ToArray())
            {
                if (activePoints[key] == o) continue;
                key.RemoveFromHoverState();
                activePoints.Remove(key);
            }

            chart.Canvas.Invalidate();
        }

        protected void BuildContent()
        {
            var template = TooltipTemplate ?? defaultTemplate;
            if (!(template.CreateContent() is View view)) return;

            view.BindingContext = new TooltipBindingContext
            {
                Points = Points,
                FontFamily = FontFamily,
                FontSize = FontSize,
                TextColor = TextColor,
                FontAttributes = FontAttributes
            };

            Content = view;
        }
    }
}