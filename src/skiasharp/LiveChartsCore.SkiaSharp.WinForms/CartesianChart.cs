using LiveChartsCore.Context;
using LiveChartsCore.SkiaSharpView.Drawing;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace LiveChartsCore.SkiaSharpView.WinForms
{
    public class CartesianChart: Chart, ICartesianChartView<SkiaSharpDrawingContext>
    {
        CartesianChart<SkiaSharpDrawingContext> ICartesianChartView<SkiaSharpDrawingContext>.Core => (CartesianChart<SkiaSharpDrawingContext>)core;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<ICartesianSeries<SkiaSharpDrawingContext>> Series { get; set; } = new List<ICartesianSeries<SkiaSharpDrawingContext>>();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<IAxis<SkiaSharpDrawingContext>> XAxes { get; set; } = new List<Axis>();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<IAxis<SkiaSharpDrawingContext>> YAxes { get; set; } = new List<Axis>();

        protected override void InitializeCore()
        {
            core = new CartesianChart<SkiaSharpDrawingContext>(this, LiveChartsSK.DefaultPlatformBuilder, motionCanvas.CanvasCore);
            //legend = Template.FindName("legend", this) as IChartLegend<SkiaSharpDrawingContext>;
            //tooltip = Template.FindName("tooltip", this) as IChartTooltip<SkiaSharpDrawingContext>;
            core.Update();
        }
    }

}
