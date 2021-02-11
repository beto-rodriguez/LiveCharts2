using LiveChartsCore.Context;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharp.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace LiveChartsCore.WinForms
{
    public partial class CartesianChart : UserControl, IChartView<SkiaDrawingContext>
    {
        protected ChartCore<SkiaDrawingContext> core;
        private IEnumerable<ISeries<SkiaDrawingContext>> series = new List<ISeries<SkiaDrawingContext>>();
        private IList<IAxis<SkiaDrawingContext>> xAxes = new List<IAxis<SkiaDrawingContext>>();
        private IList<IAxis<SkiaDrawingContext>> yAxes = new List<IAxis<SkiaDrawingContext>>();
        private Margin drawMargin;

        public CartesianChart()
        {
            InitializeComponent();
        }

        ChartCore<SkiaDrawingContext> IChartView<SkiaDrawingContext>.Core => core;
        public Canvas<SkiaDrawingContext> CoreCanvas => naturalGeometries1.CanvasCore;

        System.Drawing.SizeF IChartView<SkiaDrawingContext>.ControlSize
        {
            get
            {
                return new System.Drawing.SizeF { Width = Width, Height = Height };
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<ISeries<SkiaDrawingContext>> Series { get => series; set { series = value; OnDataChanged(); } }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IList<IAxis<SkiaDrawingContext>> XAxes { get => xAxes; set { xAxes = value; OnDataChanged(); } }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IList<IAxis<SkiaDrawingContext>> YAxes { get => yAxes; set { yAxes = value; OnDataChanged(); } }

        public LegendPosition LegendPosition { get; set; }

        public LegendOrientation LegendOrientation { get; set; }

        public IChartLegend<SkiaDrawingContext> Legend => throw new NotImplementedException();

        public Margin DrawMargin { get => drawMargin; set { drawMargin = value; OnDataChanged(); } }

        public TimeSpan AnimationsSpeed { get; set; } = TimeSpan.FromMilliseconds(500);

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<float, float> EasingFunction { get; set; } = EasingFunctions.QuadraticIn;


        public TooltipFindingStrategy TooltipFindingStrategy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IChartTooltip<SkiaDrawingContext> Tooltip => throw new NotImplementedException();

        public TooltipPosition TooltipPosition { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private void UserControl1_Load(object sender, EventArgs e)
        {
            core = new ChartCore<SkiaDrawingContext>(this, naturalGeometries1.CanvasCore);
            core.Update();
        }

        private void CartesianChart_Resize(object sender, EventArgs e)
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
