using LiveChartsCore.Context;
using LiveChartsCore.SkiaSharpView.Drawing;
using System.Collections.Generic;
using System.ComponentModel;

namespace LiveChartsCore.SkiaSharpView.WinForms
{
    public class PieChart: Chart, IPieChartView<SkiaSharpDrawingContext>
    {
        PieChart<SkiaSharpDrawingContext> IPieChartView<SkiaSharpDrawingContext>.Core => (PieChart<SkiaSharpDrawingContext>)core;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<IPieSeries<SkiaSharpDrawingContext>> Series { get; set; }

        protected override void InitializeCore()
        {
            core = new PieChart<SkiaSharpDrawingContext>(this, LiveChartsSK.DefaultPlatformBuilder, motionCanvas.CanvasCore);
            //legend = Template.FindName("legend", this) as IChartLegend<SkiaSharpDrawingContext>;
            //tooltip = Template.FindName("tooltip", this) as IChartTooltip<SkiaSharpDrawingContext>;
            core.Update();
        }
    }

}
