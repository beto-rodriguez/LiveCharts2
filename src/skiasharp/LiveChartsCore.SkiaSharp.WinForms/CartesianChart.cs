using LiveChartsCore.Context;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace LiveChartsCore.SkiaSharpView.WinForms
{
    public partial class CartesianChart : UserControl, ICartesianChartView<SkiaSharpDrawingContext>
    {
        protected CartesianChart<SkiaSharpDrawingContext> core;
        private IEnumerable<ICartesianSeries<SkiaSharpDrawingContext>> series = new List<ICartesianSeries<SkiaSharpDrawingContext>>();
        private IEnumerable<IAxis<SkiaSharpDrawingContext>> xAxes = new List<IAxis<SkiaSharpDrawingContext>>();
        private IEnumerable<IAxis<SkiaSharpDrawingContext>> yAxes = new List<IAxis<SkiaSharpDrawingContext>>();
        private Margin drawMargin;

        public CartesianChart()
        {
            LiveChartsSkiaSharp.Register();
            InitializeComponent();
        }

        CartesianChart<SkiaSharpDrawingContext> ICartesianChartView<SkiaSharpDrawingContext>.Core => core;
        public Canvas<SkiaSharpDrawingContext> CoreCanvas => motionCanvas.CanvasCore;

        System.Drawing.SizeF IChartView<SkiaSharpDrawingContext>.ControlSize
        {
            get
            {
                return new System.Drawing.SizeF { Width = Width, Height = Height };
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<ICartesianSeries<SkiaSharpDrawingContext>> Series { get => series; set { series = value; OnDataChanged(); } }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<IAxis<SkiaSharpDrawingContext>> XAxes { get => xAxes; set { xAxes = value; OnDataChanged(); } }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<IAxis<SkiaSharpDrawingContext>> YAxes { get => yAxes; set { yAxes = value; OnDataChanged(); } }

        public LegendPosition LegendPosition { get; set; }

        public LegendOrientation LegendOrientation { get; set; }

        public IChartLegend<SkiaSharpDrawingContext> Legend => null;

        public Margin DrawMargin { get => drawMargin; set { drawMargin = value; OnDataChanged(); } }

        public TimeSpan AnimationsSpeed { get; set; } = TimeSpan.FromMilliseconds(500);

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<float, float> EasingFunction { get; set; } = EasingFunctions.QuadraticIn;


        public TooltipFindingStrategy TooltipFindingStrategy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IChartTooltip<SkiaSharpDrawingContext> Tooltip => throw new NotImplementedException();

        public TooltipPosition TooltipPosition { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private void OnLoaded(object sender, EventArgs e)
        {
            core = new CartesianChart<SkiaSharpDrawingContext>(this, motionCanvas.CanvasCore);
            core.Update();
        }

        private void OnResized(object sender, EventArgs e)
        {
            if (core == null) return;
            core.Update();
        }

        private void OnDataChanged()
        {
            if (core == null) return;
            core.Update();
        }
    }
}
