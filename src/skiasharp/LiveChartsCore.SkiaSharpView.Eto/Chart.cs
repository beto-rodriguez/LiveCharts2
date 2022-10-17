// The MIT License(MIT)
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

using System;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.Motion;
using LiveChartsCore.VisualElements;

namespace LiveChartsCore.SkiaSharpView.Eto;

/// <inheritdoc cref="IChartView" />
public abstract class Chart : Panel, IChartView<SkiaSharpDrawingContext>
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
    private Font _tooltipFont = Fonts.Sans(11);
    private Color _tooltipBackColor = Color.FromArgb(250, 250, 250);
    private Font _legendFont = Fonts.Sans(11);
    private Color _legendBackColor = Color.FromArgb(255, 255, 255);
    private Color _legendTextColor = Color.FromArgb(35, 35, 35);
    private Color _tooltipTextColor = SystemColors.ControlText;
    private VisualElement<SkiaSharpDrawingContext>? _title;

    /// <summary>
    /// Initializes a new instance of the <see cref="Chart"/> class.
    /// </summary>
    /// <param name="tooltip">The default tool tip control.</param>
    /// <param name="legend">The default legend.</param>
    /// <exception cref="MotionCanvas"></exception>
    protected Chart(IChartTooltip<SkiaSharpDrawingContext>? tooltip, IChartLegend<SkiaSharpDrawingContext>? legend)
    {
        if (!LiveCharts.IsConfigured) LiveCharts.Configure(LiveChartsSkiaSharp.DefaultPlatformBuilder);

        if (tooltip is not null) this.tooltip = tooltip;
        if (legend is not null) this.legend = legend;

        motionCanvas = new MotionCanvas { MaxFps = 65 };
        motionCanvas.SizeChanged += OnResized;

        UpdateLegendLayout();

        BackgroundColor = Colors.White;

        InitializeCore();

        if (core is null) throw new Exception("Core not found!");
        core.Measuring += OnCoreMeasuring;
        core.UpdateStarted += OnCoreUpdateStarted;
        core.UpdateFinished += OnCoreUpdateFinished;

        var c = motionCanvas;
        c.MouseMove += OnMouseMove;
        c.MouseLeave += Chart_MouseLeave;

        Load += Chart_Load;
    }

    private void UpdateLegendLayout()
    {
        var legend = (Control)this.legend;

        var layout = new DynamicLayout();

        if (LegendPosition is LegendPosition.Left or LegendPosition.Right)
            _ = layout.BeginHorizontal();
        else
            _ = layout.BeginVertical();

        if (LegendPosition is LegendPosition.Top)
            layout.AddCentered(legend, horizontalCenter: true);
        if (LegendPosition is LegendPosition.Left)
            layout.AddCentered(legend, verticalCenter: true);

        _ = layout.Add(motionCanvas, xscale: true, yscale: true);

        if (LegendPosition is LegendPosition.Bottom)
            layout.AddCentered(legend, horizontalCenter: true);
        if (LegendPosition is LegendPosition.Right)
            layout.AddCentered(legend, verticalCenter: true);

        Content = layout;
    }

    #region events

    /// <inheritdoc cref="IChartView{TDrawingContext}.Measuring" />
    public event ChartEventHandler<SkiaSharpDrawingContext>? Measuring;

    /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateStarted" />
    public event ChartEventHandler<SkiaSharpDrawingContext>? UpdateStarted;

    /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateFinished" />
    public event ChartEventHandler<SkiaSharpDrawingContext>? UpdateFinished;

    /// <inheritdoc cref="IChartView.DataPointerDown" />
    public event ChartPointsHandler? DataPointerDown;

    /// <inheritdoc cref="IChartView.ChartPointPointerDown" />
    public event ChartPointHandler? ChartPointPointerDown;

    #endregion

    #region properties

    /// <inheritdoc cref="IChartView.DesignerMode" />
    bool IChartView.DesignerMode => false;

    /// <inheritdoc cref="IChartView.CoreChart" />
    public IChart CoreChart => core ?? throw new Exception("Core not set yet.");

    LvcColor IChartView.BackColor
    {
        get => new((byte)BackgroundColor.Rb, (byte)BackgroundColor.Gb, (byte)BackgroundColor.Bb, (byte)BackgroundColor.Ab);
        set => BackgroundColor = Color.FromArgb(value.R, value.G, value.B, value.A);
    }

    LvcSize IChartView.ControlSize =>
        // return the full control size as a workaround when the legend is not set.
        // for some reason Eto.Forms has not loaded the correct size at this point when the control loads.
        LegendPosition == LegendPosition.Hidden
            ? new LvcSize() { Width = ClientSize.Width, Height = ClientSize.Height }
            : new LvcSize() { Width = motionCanvas.Width, Height = motionCanvas.Height };

    /// <inheritdoc cref="IChartView{TDrawingContext}.CoreCanvas" />
    public MotionCanvas<SkiaSharpDrawingContext> CoreCanvas => motionCanvas.CanvasCore;

    /// <inheritdoc cref="IChartView.DrawMargin" />
    public Margin? DrawMargin { get => _drawMargin; set { _drawMargin = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView.SyncContext" />
    public object SyncContext { get => CoreCanvas.Sync; set { CoreCanvas.Sync = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView.AnimationsSpeed" />
    public TimeSpan AnimationsSpeed { get; set; } = LiveCharts.CurrentSettings.DefaultAnimationsSpeed;

    /// <inheritdoc cref="IChartView.AnimationsSpeed" />
    public Func<float, float>? EasingFunction { get; set; } = LiveCharts.CurrentSettings.DefaultEasingFunction;

    /// <inheritdoc cref="IChartView.LegendPosition" />
    public LegendPosition LegendPosition { get => _legendPosition; set { _legendPosition = value; UpdateLegendLayout(); OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView.LegendOrientation" />
    public LegendOrientation LegendOrientation { get => _legendOrientation; set { _legendOrientation = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the default legend font.
    /// </summary>
    /// <value>
    /// The legend font.
    /// </value>
    public Font LegendFont { get => _legendFont; set { _legendFont = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the default color of the legend text.
    /// </summary>
    /// <value>
    /// The color of the legend back.
    /// </value>
    public Color LegendTextColor { get => _legendTextColor; set { _legendTextColor = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the default color of the legend back.
    /// </summary>
    /// <value>
    /// The color of the legend back.
    /// </value>
    public Color LegendBackColor { get => _legendBackColor; set { _legendBackColor = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView{TDrawingContext}.Legend" />
    public IChartLegend<SkiaSharpDrawingContext>? Legend => legend;

    /// <inheritdoc cref="IChartView.LegendPosition" />
    public TooltipPosition TooltipPosition { get => _tooltipPosition; set { _tooltipPosition = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the default tool tip font.
    /// </summary>
    /// <value>
    /// The tool tip font.
    /// </value>
    public Font TooltipFont { get => _tooltipFont; set { _tooltipFont = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the color of the tool tip text.
    /// </summary>
    /// <value>
    /// The color of the tool tip text.
    /// </value>
    public Color TooltipTextColor { get => _tooltipTextColor; set { _tooltipTextColor = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the color of the default tool tip back.
    /// </summary>
    /// <value>
    /// The color of the tool tip back.
    /// </value>
    public Color TooltipBackColor { get => _tooltipBackColor; set { _tooltipBackColor = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView{TDrawingContext}.Tooltip" />
    public IChartTooltip<SkiaSharpDrawingContext>? Tooltip => tooltip;

    /// <inheritdoc cref="IChartView{TDrawingContext}.AutoUpdateEnabled" />
    public bool AutoUpdateEnabled { get; set; } = true;

    /// <inheritdoc cref="IChartView.UpdaterThrottler" />
    public TimeSpan UpdaterThrottler
    {
        get => core?.UpdaterThrottler ?? throw new Exception("core not set yet.");
        set
        {
            if (core is null) throw new Exception("core not set yet.");
            core.UpdaterThrottler = value;
        }
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.Title"/>
    public VisualElement<SkiaSharpDrawingContext>? Title { get => _title; set { _title = value; OnPropertyChanged(); } }

    #endregion

    /// <inheritdoc cref="IChartView{TDrawingContext}.ShowTooltip(IEnumerable{ChartPoint})"/>
    public void ShowTooltip(IEnumerable<ChartPoint> points)
    {
        if (tooltip is null || core is null) return;

        tooltip.Show(points, core);
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.HideTooltip"/>
    public void HideTooltip()
    {
        if (tooltip is null || core is null) return;

        core.ClearTooltipData();
        tooltip.Hide();
    }

    internal Point GetCanvasPosition()
    {
        return motionCanvas.Location;
    }

    /// <inheritdoc cref="IChartView.SetTooltipStyle(LvcColor, LvcColor)"/>
    public void SetTooltipStyle(LvcColor background, LvcColor textColor)
    {
        TooltipBackColor = Color.FromArgb(background.R, background.G, background.B, background.A);
        TooltipTextColor = Color.FromArgb(textColor.R, textColor.G, textColor.B, textColor.A);
    }

    void IChartView.InvokeOnUIThread(Action action)
    {
        Application.Instance.InvokeAsync(action).Wait();
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
        core?.Update();
    }

    /// <inheritdoc cref="Control.OnUnLoad(EventArgs)"/>
    protected override void OnUnLoad(EventArgs e)
    {
        if (tooltip is IDisposable disposableTooltip)
        {
            //            disposableTooltip.Dispose();

            //           tooltip = null;
        }

        base.OnUnLoad(e);

        core?.Unload();
        OnUnloading();
    }

    /// <summary>
    /// Called when the control is being unloaded.
    /// </summary>
    protected virtual void OnUnloading() { }

    /// <inheritdoc cref="Control.OnLoadComplete(EventArgs)"/>
    protected override void OnLoadComplete(EventArgs e)
    {
        base.OnLoadComplete(e);
        core?.Load();
    }

    private void OnResized(object? sender, EventArgs e)
    {
        if (core is null) return;
        core.Update();
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

    /// <summary>
    /// Called when the mouse goes down.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnMouseMove(object? sender, MouseEventArgs e)
    {
        core?.InvokePointerMove(new LvcPoint(e.Location.X, e.Location.Y));
    }

    /// <summary>
    /// Called when the mouse leaves the control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void Chart_MouseLeave(object? sender, EventArgs e)
    {
        if (this.tooltip is Window tooltip)
        {
            var window = tooltip.Size + tooltip.Location;

            if (window.Contains((Point)Mouse.Position)) // mouse over tooltip ?
                return; // dont hide it 
        }

        HideTooltip();
        core?.InvokePointerLeft();
    }

    private void Chart_Load(object? sender, EventArgs e)
    {
        core?.Load();
    }

    void IChartView.OnDataPointerDown(IEnumerable<ChartPoint> points, LvcPoint pointer)
    {
        DataPointerDown?.Invoke(this, points);
        ChartPointPointerDown?.Invoke(this, points.FindClosestTo(pointer));
    }

    void IChartView.Invalidate()
    {
        CoreCanvas.Invalidate();
    }
}
