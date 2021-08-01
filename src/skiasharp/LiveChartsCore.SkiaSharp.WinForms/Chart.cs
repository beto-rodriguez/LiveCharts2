﻿// The MIT License(MIT)
//
// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace LiveChartsCore.SkiaSharpView.WinForms
{
    /// <inheritdoc cref="IChartView" />
    public abstract class Chart : UserControl, IChartView<SkiaSharpDrawingContext>
    {
        /// <summary>
        /// The core
        /// </summary>
        protected Chart<SkiaSharpDrawingContext>? core;

        /// <summary>
        /// The legend
        /// </summary>
        protected IChartLegend<SkiaSharpDrawingContext> legend = new DefaultLegend();

        /// <summary>
        /// The tool tip
        /// </summary>
        protected IChartTooltip<SkiaSharpDrawingContext> tooltip = new DefaultTooltip();

        /// <summary>
        /// The motion canvas
        /// </summary>
        protected MotionCanvas motionCanvas;

        private LegendPosition _legendPosition = LiveCharts.CurrentSettings.DefaultLegendPosition;
        private LegendOrientation _legendOrientation = LiveCharts.CurrentSettings.DefaultLegendOrientation;
        private Margin? _drawMargin = null;
        private TooltipPosition _tooltipPosition = LiveCharts.CurrentSettings.DefaultTooltipPosition;
        private Font _tooltipFont = new(new FontFamily("Trebuchet MS"), 11, FontStyle.Regular);
        private Color _tooltipBackColor = Color.FromArgb(255, 250, 250, 250);
        private Font _legendFont = new(new FontFamily("Trebuchet MS"), 11, FontStyle.Regular);
        private Color _legendBackColor = Color.FromArgb(255, 250, 250, 250);
        private Color _legendTextColor = Color.FromArgb(255, 250, 250, 250);
        private Color _tooltipTextColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="Chart"/> class.
        /// </summary>
        /// <param name="tooltip">The default tool tip control.</param>
        /// <param name="legend">The default legend.</param>
        /// <exception cref="MotionCanvas"></exception>
        protected Chart(IChartTooltip<SkiaSharpDrawingContext>? tooltip, IChartLegend<SkiaSharpDrawingContext>? legend)
        {
            if (tooltip is not null) this.tooltip = tooltip;
            if (legend is not null) this.legend = legend;

            motionCanvas = new MotionCanvas();
            SuspendLayout();
            motionCanvas.Dock = DockStyle.Fill;
            motionCanvas.FramesPerSecond = 90D;
            motionCanvas.Location = new Point(0, 0);
            motionCanvas.Name = "motionCanvas";
            motionCanvas.Size = new Size(150, 150);
            motionCanvas.TabIndex = 0;
            motionCanvas.Resize += OnResized;
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(motionCanvas);
            var l = (Control)this.legend;
            l.Visible = false;
            l.Dock = DockStyle.Right;
            Controls.Add(l);
            Name = "CartesianChart";
            ResumeLayout(true);

            if (!LiveCharts.IsConfigured) LiveCharts.Configure(LiveChartsSkiaSharp.DefaultPlatformBuilder);

            var stylesBuilder = LiveCharts.CurrentSettings.GetTheme<SkiaSharpDrawingContext>();
            var initializer = stylesBuilder.GetVisualsInitializer();
            if (stylesBuilder.CurrentColors is null || stylesBuilder.CurrentColors.Length == 0)
                throw new Exception("Default colors are not valid");
            initializer.ApplyStyleToChart(this);

            var c = Controls[0].Controls[0];
            c.MouseMove += OnMouseMove;

            InitializeCore();

            if (core is null) throw new Exception("Core not found!");
            core.Measuring += OnCoreMeasuring;
            core.UpdateStarted += OnCoreUpdateStarted;
            core.UpdateFinished += OnCoreUpdateFinished;

            c.MouseLeave += Chart_MouseLeave;

            var s = PointStates;
        }

        #region events

        /// <inheritdoc cref="IChartView{TDrawingContext}.Measuring" />
        public event ChartEventHandler<SkiaSharpDrawingContext>? Measuring;

        /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateStarted" />
        public event ChartEventHandler<SkiaSharpDrawingContext>? UpdateStarted;

        /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateFinished" />
        public event ChartEventHandler<SkiaSharpDrawingContext>? UpdateFinished;

        #endregion

        #region properties

        /// <inheritdoc cref="IChartView.CoreChart" />
        public IChart CoreChart => core ?? throw new Exception("Core not set yet.");

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        Color IChartView.BackColor
        {
            get => BackColor;
            set => BackColor = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        SizeF IChartView.ControlSize =>
                // return the full control size as a workaround when the legend is not set.
                // for some reason WinForms has not loaded the correct size at this point when the control loads.
                LegendPosition == LegendPosition.Hidden
                    ? new SizeF() { Width = ClientSize.Width, Height = ClientSize.Height }
                    : new SizeF() { Width = motionCanvas.Width, Height = motionCanvas.Height };

        /// <inheritdoc cref="IChartView{TDrawingContext}.CoreCanvas" />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MotionCanvas<SkiaSharpDrawingContext> CoreCanvas => motionCanvas.CanvasCore;

        /// <inheritdoc cref="IChartView.DrawMargin" />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Margin? DrawMargin { get => _drawMargin; set { _drawMargin = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IChartView.AnimationsSpeed" />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TimeSpan AnimationsSpeed { get; set; } = LiveCharts.CurrentSettings.DefaultAnimationsSpeed;

        /// <inheritdoc cref="IChartView.AnimationsSpeed" />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<float, float>? EasingFunction { get; set; } = LiveCharts.CurrentSettings.DefaultEasingFunction;

        /// <inheritdoc cref="IChartView.LegendPosition" />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public LegendPosition LegendPosition { get => _legendPosition; set { _legendPosition = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IChartView.LegendOrientation" />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public LegendOrientation LegendOrientation { get => _legendOrientation; set { _legendOrientation = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets the default legend font.
        /// </summary>
        /// <value>
        /// The legend font.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Font LegendFont { get => _legendFont; set { _legendFont = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets the default color of the legend text.
        /// </summary>
        /// <value>
        /// The color of the legend back.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color LegendTextColor { get => _legendTextColor; set { _legendTextColor = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets the default color of the legend back.
        /// </summary>
        /// <value>
        /// The color of the legend back.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color LegendBackColor { get => _legendBackColor; set { _legendBackColor = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IChartView{TDrawingContext}.Legend" />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IChartLegend<SkiaSharpDrawingContext>? Legend => legend;

        /// <inheritdoc cref="IChartView.LegendPosition" />
        public TooltipPosition TooltipPosition { get => _tooltipPosition; set { _tooltipPosition = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets the default tool tip font.
        /// </summary>
        /// <value>
        /// The tool tip font.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Font TooltipFont { get => _tooltipFont; set { _tooltipFont = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets the color of the tool tip text.
        /// </summary>
        /// <value>
        /// The color of the tool tip text.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color TooltipTextColor { get => _tooltipTextColor; set { _tooltipTextColor = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets the color of the default tool tip back.
        /// </summary>
        /// <value>
        /// The color of the tool tip back.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color TooltipBackColor { get => _tooltipBackColor; set { _tooltipBackColor = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IChartView{TDrawingContext}.Tooltip" />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IChartTooltip<SkiaSharpDrawingContext>? Tooltip => tooltip;

        /// <inheritdoc cref="IChartView{TDrawingContext}.PointStates" />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PointStatesDictionary<SkiaSharpDrawingContext> PointStates { get; set; } = new();

        /// <inheritdoc cref="IChartView{TDrawingContext}.AutoUpdateEnabled" />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AutoUpdateEnabled { get; set; } = true;

        /// <inheritdoc cref="IChartView.UpdaterThrottler" />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TimeSpan UpdaterThrottler
        {
            get => core?.UpdaterThrottler ?? throw new Exception("core not set yet.");
            set
            {
                if (core is null) throw new Exception("core not set yet.");
                core.UpdaterThrottler = value;
            }
        }

        #endregion

        /// <inheritdoc cref="IChartView{TDrawingContext}.ShowTooltip(IEnumerable{TooltipPoint})"/>
        public void ShowTooltip(IEnumerable<TooltipPoint> points)
        {
            if (tooltip is null || core is null) return;

            tooltip.Show(points, core);
        }

        /// <inheritdoc cref="IChartView{TDrawingContext}.HideTooltip"/>
        public void HideTooltip()
        {
            if (tooltip is null || core is null) return;

            foreach (var state in PointStates.GetStates())
            {
                if (!state.IsHoverState) continue;
                if (state.Fill is not null) state.Fill.ClearGeometriesFromPaintTask(core.Canvas);
                if (state.Stroke is not null) state.Stroke.ClearGeometriesFromPaintTask(core.Canvas);
            }

            tooltip.Hide();
        }

        /// <inheritdoc cref="IChartView.SetTooltipStyle(Color, Color)"/>
        public void SetTooltipStyle(Color background, Color textColor)
        {
            TooltipBackColor = background;
            TooltipTextColor = textColor;
        }

        /// <summary>
        /// Initializes the core.
        /// </summary>
        /// <returns></returns>
        protected abstract void InitializeCore();

        /// <summary>
        /// Called when a property changes.
        /// </summary>
        /// <returns></returns>
        protected void OnPropertyChanged()
        {
            if (core is null) return;
            core.Update();
        }

        /// <summary>
        /// Raises the <see cref="E:HandleDestroyed" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (tooltip is IDisposable disposableTooltip) disposableTooltip.Dispose();
            base.OnHandleDestroyed(e);
        }

        private void OnResized(object? sender, EventArgs e)
        {
            if (core is null) return;
            core.Update();
        }

        private void OnMouseMove(object? sender, MouseEventArgs e)
        {
            core?.InvokePointerMove(new PointF(e.Location.X, e.Location.Y));
        }

        private void OnCoreUpdateFinished(IChartView<SkiaSharpDrawingContext> chart)
        {
            UpdateFinished?.Invoke(this);
        }

        private void OnCoreUpdateStarted(IChartView<SkiaSharpDrawingContext> chart)
        {
            UpdateStarted?.Invoke(this);
        }

        private void OnCoreMeasuring(IChartView<SkiaSharpDrawingContext> chart)
        {
            Measuring?.Invoke(this);
        }

        private void Chart_MouseLeave(object? sender, EventArgs e)
        {
            HideTooltip();
            core?.InvokePointerLeft();
        }
    }
}
