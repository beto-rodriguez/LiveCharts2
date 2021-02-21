using LiveChartsCore.Context;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace LiveChartsCore.SkiaSharpView.WinForms
{
    public partial class CartesianChart : UserControl, ICartesianChartView<SkiaDrawingContext>
    {
        protected CartesianChartCore<SkiaDrawingContext> core;
        private IEnumerable<ISeries<SkiaDrawingContext>> series = new List<ISeries<SkiaDrawingContext>>();
        private IEnumerable<IAxis<SkiaDrawingContext>> xAxes = new List<IAxis<SkiaDrawingContext>>();
        private IEnumerable<IAxis<SkiaDrawingContext>> yAxes = new List<IAxis<SkiaDrawingContext>>();
        private Margin drawMargin;

        public CartesianChart()
        {
            InitializeComponent();
        }

        CartesianChartCore<SkiaDrawingContext> ICartesianChartView<SkiaDrawingContext>.Core => core;
        public Canvas<SkiaDrawingContext> CoreCanvas => motionCanvas.CanvasCore;

        System.Drawing.SizeF ICartesianChartView<SkiaDrawingContext>.ControlSize
        {
            get
            {
                return new System.Drawing.SizeF { Width = Width, Height = Height };
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<ISeries<SkiaDrawingContext>> Series { get => series; set { series = value; OnDataChanged(); } }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<IAxis<SkiaDrawingContext>> XAxes { get => xAxes; set { xAxes = value; OnDataChanged(); } }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<IAxis<SkiaDrawingContext>> YAxes { get => yAxes; set { yAxes = value; OnDataChanged(); } }

        public LegendPosition LegendPosition { get; set; }

        public LegendOrientation LegendOrientation { get; set; }

        public IChartLegend<SkiaDrawingContext> Legend => null;

        public Margin DrawMargin { get => drawMargin; set { drawMargin = value; OnDataChanged(); } }

        public TimeSpan AnimationsSpeed { get; set; } = TimeSpan.FromMilliseconds(500);

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<float, float> EasingFunction { get; set; } = EasingFunctions.QuadraticIn;


        public TooltipFindingStrategy TooltipFindingStrategy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IChartTooltip<SkiaDrawingContext> Tooltip => throw new NotImplementedException();

        public TooltipPosition TooltipPosition { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private void OnLoaded(object sender, EventArgs e)
        {
            core = new CartesianChartCore<SkiaDrawingContext>(this, motionCanvas.CanvasCore);
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
