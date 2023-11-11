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

// Ignore Spelling: Crosshair Subticks Subseparators

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Helpers;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Motion;

namespace LiveChartsCore;

/// <summary>
/// Defines an Axis in a Cartesian chart.
/// </summary>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
/// <typeparam name="TTextGeometry">The type of the text geometry.</typeparam>
/// <typeparam name="TLineGeometry">The type of the line geometry.</typeparam>
public abstract class CoreAxis<TDrawingContext, TTextGeometry, TLineGeometry>
    : ChartElement<TDrawingContext>, ICartesianAxis<TDrawingContext>, IPlane<TDrawingContext>
        where TDrawingContext : DrawingContext
        where TTextGeometry : ILabelGeometry<TDrawingContext>, new()
        where TLineGeometry : class, ILineGeometry<TDrawingContext>, new()
{
    #region fields

    /// <summary>
    /// The active separators
    /// </summary>
    protected internal readonly Dictionary<IChart, Dictionary<string, AxisVisualSeprator<TDrawingContext>>> activeSeparators = new();

    internal float _xo = 0f, _yo = 0f;
    internal LvcSize _size;
    internal AxisOrientation _orientation;
    internal AnimatableAxisBounds _animatableBounds = new();
    internal Bounds _dataBounds = new();
    internal Bounds _visibleDataBounds = new();

    private double _minStep = 0;
    private double _labelsRotation;
    private LvcRectangle _labelsDesiredSize = new(), _nameDesiredSize = new();
    private LvcSize? _possibleMaxLabelsSize = new();
    private TTextGeometry? _nameGeometry;
    private AxisPosition _position = AxisPosition.Start;
    private Func<double, string> _labeler = Labelers.Default;
    private Padding _padding = new();
    private double? _minLimit = null;
    private double? _maxLimit = null;
    private IPaint<TDrawingContext>? _namePaint;
    private double _nameTextSize = 20;
    private Padding _namePadding = new(5);
    private IPaint<TDrawingContext>? _labelsPaint;
    private double _unitWidth = 1;
    private double _textSize = 16;
    private IPaint<TDrawingContext>? _separatorsPaint;
    private IPaint<TDrawingContext>? _subseparatorsPaint;
    private bool _drawTicksPath;
    private ILineGeometry<TDrawingContext>? _ticksPath;
    private IPaint<TDrawingContext>? _ticksPaint;
    private IPaint<TDrawingContext>? _subticksPaint;
    private IPaint<TDrawingContext>? _zeroPaint;
    private ILineGeometry<TDrawingContext>? _zeroLine;
    private ILineGeometry<TDrawingContext>? _crosshairLine;
    private ILabelGeometry<TDrawingContext>? _crosshairLabel;
    private IPaint<TDrawingContext>? _crosshairPaint;
    private IPaint<TDrawingContext>? _crosshairLabelsPaint;
    private LvcColor? _crosshairLabelsBackground;
    private bool _showSeparatorLines = true;
    private bool _isVisible = true;
    private bool _isInverted;
    private bool _separatorsAtCenter = true;
    private bool _ticksAtCenter = true;
    private bool _forceStepToMin;
    private bool _crosshairSnapEnabled;
    private readonly float _tickLength = 6f;
    private int _subseparatorsCount = 3;
    private Align? _labelsAlignment;
    private bool _inLineNamePlacement;
    private IEnumerable<double>? _customSeparators;
    private int _stepCount;
    internal double? _logBase;

    #endregion

    #region properties

    float ICartesianAxis.Xo { get => _xo; set => _xo = value; }
    float ICartesianAxis.Yo { get => _yo; set => _yo = value; }
    LvcSize ICartesianAxis.Size { get => _size; set => _size = value; }
    LvcRectangle ICartesianAxis.LabelsDesiredSize { get => _labelsDesiredSize; set => _labelsDesiredSize = value; }
    LvcSize ICartesianAxis.PossibleMaxLabelSize => _possibleMaxLabelsSize ?? (_possibleMaxLabelsSize = GetPossibleMaxLabelSize()).Value;
    LvcRectangle ICartesianAxis.NameDesiredSize { get => _nameDesiredSize; set => _nameDesiredSize = value; }

    /// <inheritdoc cref="IPlane.DataBounds"/>
    public Bounds DataBounds => _dataBounds;

    /// <inheritdoc cref="IPlane.VisibleDataBounds"/>
    public Bounds VisibleDataBounds => _visibleDataBounds;

    AnimatableAxisBounds IPlane.ActualBounds => _animatableBounds;

    /// <inheritdoc cref="IPlane.Name"/>
    public string? Name { get; set; } = null;

    /// <inheritdoc cref="IPlane.NameTextSize"/>
    public double NameTextSize { get => _nameTextSize; set => SetProperty(ref _nameTextSize, value); }

    /// <inheritdoc cref="IPlane.NamePadding"/>
    public Padding NamePadding { get => _namePadding; set => SetProperty(ref _namePadding, value); }

    /// <inheritdoc cref="ICartesianAxis.LabelsAlignment"/>
    public Align? LabelsAlignment { get => _labelsAlignment; set => SetProperty(ref _labelsAlignment, value); }

    /// <inheritdoc cref="ICartesianAxis.Orientation"/>
    public AxisOrientation Orientation => _orientation;

    /// <inheritdoc cref="ICartesianAxis.Padding"/>
    public Padding Padding { get => _padding; set => SetProperty(ref _padding, value); }

    /// <inheritdoc cref="IPlane.Labeler"/>
    public Func<double, string> Labeler { get => _labeler; set => SetProperty(ref _labeler, value); }

    /// <inheritdoc cref="IPlane.MinStep"/>
    public double MinStep { get => _minStep; set => SetProperty(ref _minStep, value); }

    /// <inheritdoc cref="IPlane.ForceStepToMin"/>
    public bool ForceStepToMin { get => _forceStepToMin; set => SetProperty(ref _forceStepToMin, value); }

    /// <inheritdoc cref="IPlane.MinLimit"/>
    public double? MinLimit { get => _minLimit; set => SetProperty(ref _minLimit, value); }

    /// <inheritdoc cref="IPlane.MaxLimit"/>
    public double? MaxLimit { get => _maxLimit; set => SetProperty(ref _maxLimit, value); }

    /// <inheritdoc cref="IPlane.UnitWidth"/>
    public double UnitWidth { get => _unitWidth; set => SetProperty(ref _unitWidth, value); }

    /// <inheritdoc cref="ICartesianAxis.Position"/>
    public AxisPosition Position { get => _position; set => SetProperty(ref _position, value); }

    /// <inheritdoc cref="IPlane.LabelsRotation"/>
    public double LabelsRotation { get => _labelsRotation; set => SetProperty(ref _labelsRotation, value); }

    /// <inheritdoc cref="IPlane.TextSize"/>
    public double TextSize { get => _textSize; set => SetProperty(ref _textSize, value); }

    /// <inheritdoc cref="IPlane.Labels"/>
    public IList<string>? Labels { get; set; }

    /// <inheritdoc cref="IPlane.ShowSeparatorLines"/>
    public bool ShowSeparatorLines { get => _showSeparatorLines; set => SetProperty(ref _showSeparatorLines, value); }

    /// <inheritdoc cref="IPlane.CustomSeparators"/>
    public IEnumerable<double>? CustomSeparators { get => _customSeparators; set => SetProperty(ref _customSeparators, value); }

    /// <inheritdoc cref="IPlane.IsVisible"/>
    public bool IsVisible { get => _isVisible; set => SetProperty(ref _isVisible, value); }

    /// <inheritdoc cref="IPlane.IsInverted"/>
    public bool IsInverted { get => _isInverted; set => SetProperty(ref _isInverted, value); }

    /// <inheritdoc cref="ICartesianAxis.SeparatorsAtCenter"/>
    public bool SeparatorsAtCenter { get => _separatorsAtCenter; set => SetProperty(ref _separatorsAtCenter, value); }

    /// <inheritdoc cref="ICartesianAxis.TicksAtCenter"/>
    public bool TicksAtCenter { get => _ticksAtCenter; set => SetProperty(ref _ticksAtCenter, value); }

    /// <inheritdoc cref="IPlane{TDrawingContext}.NamePaint"/>
    public IPaint<TDrawingContext>? NamePaint
    {
        get => _namePaint;
        set => SetPaintProperty(ref _namePaint, value);
    }

    /// <inheritdoc cref="IPlane{TDrawingContext}.LabelsPaint"/>
    public IPaint<TDrawingContext>? LabelsPaint
    {
        get => _labelsPaint;
        set => SetPaintProperty(ref _labelsPaint, value);
    }

    /// <inheritdoc cref="IPlane{TDrawingContext}.SeparatorsPaint"/>
    public IPaint<TDrawingContext>? SeparatorsPaint
    {
        get => _separatorsPaint;
        set => SetPaintProperty(ref _separatorsPaint, value, true);
    }

    /// <inheritdoc cref="ICartesianAxis{TDrawingContext}.SubseparatorsPaint"/>
    public IPaint<TDrawingContext>? SubseparatorsPaint
    {
        get => _subseparatorsPaint;
        set => SetPaintProperty(ref _subseparatorsPaint, value, true);
    }

    /// <inheritdoc cref="ICartesianAxis{TDrawingContext}.SubseparatorsCount"/>
    public int SubseparatorsCount { get => _subseparatorsCount; set => SetProperty(ref _subseparatorsCount, value); }

    /// <inheritdoc cref="ICartesianAxis{TDrawingContext}.DrawTicksPath"/>
    public bool DrawTicksPath { get => _drawTicksPath; set => SetProperty(ref _drawTicksPath, value); }

    /// <inheritdoc cref="ICartesianAxis{TDrawingContext}.TicksPaint"/>
    public IPaint<TDrawingContext>? TicksPaint
    {
        get => _ticksPaint;
        set => SetPaintProperty(ref _ticksPaint, value, true);
    }

    /// <inheritdoc cref="ICartesianAxis{TDrawingContext}.SubticksPaint"/>
    public IPaint<TDrawingContext>? SubticksPaint
    {
        get => _subticksPaint;
        set => SetPaintProperty(ref _subticksPaint, value, true);
    }

    /// <inheritdoc cref="ICartesianAxis{TDrawingContext}.ZeroPaint"/>
    public IPaint<TDrawingContext>? ZeroPaint
    {
        get => _zeroPaint;
        set => SetPaintProperty(ref _zeroPaint, value, true);
    }

    /// <inheritdoc cref="ICartesianAxis{TDrawingContext}.CrosshairPaint"/>
    public IPaint<TDrawingContext>? CrosshairPaint
    {
        get => _crosshairPaint;
        set => SetPaintProperty(ref _crosshairPaint, value, true);
    }

    /// <inheritdoc cref="ICartesianAxis{TDrawingContext}.CrosshairLabelsPaint"/>
    public IPaint<TDrawingContext>? CrosshairLabelsPaint
    {
        get => _crosshairLabelsPaint;
        set => SetPaintProperty(ref _crosshairLabelsPaint, value);
    }

    /// <inheritdoc cref="ICartesianAxis{TDrawingContext}.CrosshairLabelsBackground"/>
    public LvcColor? CrosshairLabelsBackground
    {
        get => _crosshairLabelsBackground;
        set => SetProperty(ref _crosshairLabelsBackground, value);
    }

    /// <inheritdoc cref="ICartesianAxis{TDrawingContext}.CrosshairPadding"/>
    public Padding? CrosshairPadding { get; set; }

    /// <inheritdoc cref="ICartesianAxis{TDrawingContext}.CrosshairSnapEnabled" />
    public bool CrosshairSnapEnabled { get => _crosshairSnapEnabled; set => SetProperty(ref _crosshairSnapEnabled, value); }

    /// <summary>
    /// 
    /// </summary>
    [Obsolete("Renamed to LabelsPaint")]
    public IPaint<TDrawingContext>? TextBrush { get => LabelsPaint; set => LabelsPaint = value; }

    /// <summary>
    /// 
    /// </summary>
    [Obsolete("Renamed to SeparatorsPaint")]
    public IPaint<TDrawingContext>? SeparatorsBrush { get => SeparatorsPaint; set => SeparatorsPaint = value; }

    /// <inheritdoc cref="IPlane.AnimationsSpeed"/>
    public TimeSpan? AnimationsSpeed { get; set; }

    /// <inheritdoc cref="IPlane.EasingFunction"/>
    public Func<float, float>? EasingFunction { get; set; }

    /// <inheritdoc cref="ICartesianAxis.MinZoomDelta"/>
    public double? MinZoomDelta { get; set; }

    /// <inheritdoc cref="ICartesianAxis.MinZoomDelta"/>
    public bool InLineNamePlacement { get => _inLineNamePlacement; set => SetProperty(ref _inLineNamePlacement, value); }

    /// <inheritdoc cref="ICartesianAxis.SharedWith"/>
    public IEnumerable<ICartesianAxis>? SharedWith { get; set; }

    #endregion

    /// <inheritdoc cref="ICartesianAxis.Initialized"/>
    public event Action<ICartesianAxis>? Initialized;

    /// <inheritdoc cref="ChartElement{TDrawingContext}.Invalidate(Chart{TDrawingContext})"/>
    public override void Invalidate(Chart<TDrawingContext> chart)
    {
        _stepCount = 0;

        var cartesianChart = (CartesianChart<TDrawingContext>)chart;

        var controlSize = cartesianChart.ControlSize;
        var drawLocation = cartesianChart.DrawMarginLocation;
        var drawMarginSize = cartesianChart.DrawMarginSize;

        var max = MaxLimit is null ? _visibleDataBounds.Max : MaxLimit.Value;
        var min = MinLimit is null ? _visibleDataBounds.Min : MinLimit.Value;

        AxisLimit.ValidateLimits(ref min, ref max);

        _animatableBounds.MaxVisibleBound = max;
        _animatableBounds.MinVisibleBound = min;

        if (!_animatableBounds.HasPreviousState)
        {
            _animatableBounds.Animate(EasingFunction ?? cartesianChart.EasingFunction, AnimationsSpeed ?? cartesianChart.AnimationsSpeed);
            _ = cartesianChart.Canvas.Trackers.Add(_animatableBounds);
        }

        var scale = this.GetNextScaler(cartesianChart);
        var actualScale = this.GetActualScaler(cartesianChart) ?? scale;
        var labeler = GetActualLabeler();

        if (NamePaint is not null)
        {
            if (NamePaint.ZIndex == 0) NamePaint.ZIndex = -1;
            cartesianChart.Canvas.AddDrawableTask(NamePaint);
        }
        if (LabelsPaint is not null)
        {
            if (LabelsPaint.ZIndex == 0) LabelsPaint.ZIndex = -0.9;
            cartesianChart.Canvas.AddDrawableTask(LabelsPaint);
        }

        var o = SeparatorsPaint?.StrokeThickness ?? 0;
        var clipping = new LvcRectangle(
            new LvcPoint(drawLocation.X - o, drawLocation.Y - o),
            new LvcSize(drawMarginSize.Width + o * 2, drawMarginSize.Height + o * 2));

        if (SubseparatorsPaint is not null)
        {
            if (SubseparatorsPaint.ZIndex == 0) SubseparatorsPaint.ZIndex = -1;
            SubseparatorsPaint.SetClipRectangle(cartesianChart.Canvas, clipping);
            cartesianChart.Canvas.AddDrawableTask(SubseparatorsPaint);
        }
        if (SeparatorsPaint is not null)
        {
            if (SeparatorsPaint.ZIndex == 0) SeparatorsPaint.ZIndex = -1;
            SeparatorsPaint.SetClipRectangle(cartesianChart.Canvas, clipping);
            cartesianChart.Canvas.AddDrawableTask(SeparatorsPaint);
        }

        var ticksClipRectangle = _orientation == AxisOrientation.X
                ? new LvcRectangle(new LvcPoint(drawLocation.X, 0), new LvcSize(drawMarginSize.Width, controlSize.Height))
                : new LvcRectangle(new LvcPoint(0, drawLocation.Y), new LvcSize(controlSize.Width, drawMarginSize.Height));

        if (TicksPaint is not null)
        {
            if (TicksPaint.ZIndex == 0) TicksPaint.ZIndex = -1;
            TicksPaint.SetClipRectangle(cartesianChart.Canvas, ticksClipRectangle);
            cartesianChart.Canvas.AddDrawableTask(TicksPaint);
        }
        if (SubticksPaint is not null)
        {
            if (SubticksPaint.ZIndex == 0) SubticksPaint.ZIndex = -1;
            SubticksPaint.SetClipRectangle(cartesianChart.Canvas, ticksClipRectangle);
            cartesianChart.Canvas.AddDrawableTask(SubticksPaint);
        }

        var lyi = drawLocation.Y;
        var lyj = drawLocation.Y + drawMarginSize.Height;
        var lxi = drawLocation.X;
        var lxj = drawLocation.X + drawMarginSize.Width;

        float xoo = 0f, yoo = 0f;

        if (_orientation == AxisOrientation.X)
        {
            yoo = _position == AxisPosition.Start
                 ? controlSize.Height - _yo
                 : _yo;
        }
        else
        {
            xoo = _position == AxisPosition.Start
                ? _xo
                : controlSize.Width - _xo;
        }

        var size = (float)TextSize;
        var r = (float)_labelsRotation;
        var hasRotation = Math.Abs(r) > 0.01f;

        if (!activeSeparators.TryGetValue(cartesianChart, out var separators))
        {
            separators = new Dictionary<string, AxisVisualSeprator<TDrawingContext>>();
            activeSeparators[cartesianChart] = separators;
        }

        if (Name is not null && NamePaint is not null)
            DrawName(cartesianChart, (float)NameTextSize, lxi, lxj, lyi, lyj);

        if (NamePaint is not null && _nameGeometry is not null)
            NamePaint.AddGeometryToPaintTask(cartesianChart.Canvas, _nameGeometry);

        var hasActivePaint =
            NamePadding is not null || SeparatorsPaint is not null || LabelsPaint is not null ||
            TicksPaint is not null || SubticksPaint is not null || SubseparatorsPaint is not null;

        var measured = new HashSet<AxisVisualSeprator<TDrawingContext>>();

        if (ZeroPaint is not null)
        {
            float x, y;
            if (_orientation == AxisOrientation.X)
            {
                x = scale.ToPixels(0);
                y = yoo;
            }
            else
            {
                x = xoo;
                y = scale.ToPixels(0);
            }

            if (ZeroPaint.ZIndex == 0) ZeroPaint.ZIndex = -1;
            ZeroPaint.SetClipRectangle(cartesianChart.Canvas, new LvcRectangle(drawLocation, drawMarginSize));
            cartesianChart.Canvas.AddDrawableTask(ZeroPaint);

            if (_zeroLine is null)
            {
                _zeroLine = new TLineGeometry();
                ZeroPaint.AddGeometryToPaintTask(cartesianChart.Canvas, _zeroLine);
                InitializeLine(_zeroLine, cartesianChart);
                UpdateSeparator(_zeroLine, x, y, lxi, lxj, lyi, lyj, UpdateMode.UpdateAndComplete);
            }

            UpdateSeparator(_zeroLine, x, y, lxi, lxj, lyi, lyj, UpdateMode.Update);
        }

        if (TicksPaint is not null && _drawTicksPath)
        {
            if (_ticksPath is null)
            {
                _ticksPath = new TLineGeometry();
                InitializeLine(_ticksPath, cartesianChart);
            }
            TicksPaint.AddGeometryToPaintTask(cartesianChart.Canvas, _ticksPath);

            if (_orientation == AxisOrientation.X)
            {
                var yp = yoo + _size.Height * 0.5f * (_position == AxisPosition.Start ? -1 : 1);
                _ticksPath.X = lxi;
                _ticksPath.X1 = lxj;
                _ticksPath.Y = yp;
                _ticksPath.Y1 = yp;
            }
            else
            {
                var xp = xoo + _size.Width * 0.5f * (_position == AxisPosition.Start ? 1 : -1);
                _ticksPath.X = xp;
                _ticksPath.X1 = xp;
                _ticksPath.Y = lyi;
                _ticksPath.Y1 = lyj;
            }

            if (!_animatableBounds.HasPreviousState) _ticksPath.CompleteTransition(null);
        }
        if (TicksPaint is not null && _ticksPath is not null && !_drawTicksPath)
            TicksPaint.RemoveGeometryFromPainTask(cartesianChart.Canvas, _ticksPath);

        float txco = 0f, tyco = 0f, sxco = 0f, syco = 0f;

        var uw = scale.MeasureInPixels(_unitWidth);
        if (!_ticksAtCenter && _orientation == AxisOrientation.X) txco = uw * 0.5f;
        if (!_ticksAtCenter && _orientation == AxisOrientation.Y) tyco = uw * 0.5f;
        if (!_separatorsAtCenter && _orientation == AxisOrientation.X) sxco = uw * 0.5f;
        if (!_separatorsAtCenter && _orientation == AxisOrientation.Y) sxco = uw * 0.5f;

        var axisTick = this.GetTick(drawMarginSize, null);
        var s = axisTick.Value;
        if (s < _minStep) s = _minStep;
        if (_forceStepToMin) s = _minStep;

        var start = Math.Truncate(min / s) * s;

        foreach (var i in EnumerateSeparators(start, max, s))
        {
            var separatorKey = Labelers.SixRepresentativeDigits(i - 1d + 1d);
            var labelContent = i < min || i > max ? string.Empty : TryGetLabelOrLogError(labeler, i - 1d + 1d);

            float x, y;
            if (_orientation == AxisOrientation.X)
            {
                x = scale.ToPixels(i);
                y = yoo;
            }
            else
            {
                x = xoo;
                y = scale.ToPixels(i);
            }

            float yc;
            float xc;

            if (_orientation == AxisOrientation.X)
            {
                xc = actualScale.ToPixels(i);
                yc = yoo;
            }
            else
            {
                xc = xoo;
                yc = actualScale.ToPixels(i);
            }

            if (!separators.TryGetValue(separatorKey, out var visualSeparator))
            {
                visualSeparator = new AxisVisualSeprator<TDrawingContext>() { Value = i };
                separators.Add(separatorKey, visualSeparator);
            }

            #region Initialize shapes

            if (SeparatorsPaint is not null && ShowSeparatorLines && visualSeparator.Separator is null)
            {
                InitializeSeparator(visualSeparator, cartesianChart);
                UpdateSeparator(
                    visualSeparator.Separator!, xc + sxco, yc + syco, lxi, lxj, lyi, lyj,
                    UpdateMode.UpdateAndComplete);
            }
            if (SubseparatorsPaint is not null && ShowSeparatorLines &&
                (visualSeparator.Subseparators is null || visualSeparator.Subseparators.Length == 0))
            {
                InitializeSubseparators(visualSeparator, cartesianChart);
                UpdateSubseparators(
                    visualSeparator.Subseparators!, actualScale, s, xc + sxco, yc + syco, lxi, lxj, lyi, lyj,
                    UpdateMode.UpdateAndComplete);
            }
            if (TicksPaint is not null && visualSeparator.Tick is null)
            {
                InitializeTick(visualSeparator, cartesianChart);
                UpdateTick(visualSeparator.Tick!, _tickLength, xc + txco, yc + tyco, UpdateMode.UpdateAndComplete);
            }
            if (SubticksPaint is not null && _subseparatorsCount > 0 &&
                (visualSeparator.Subticks is null || visualSeparator.Subticks.Length == 0))
            {
                InitializeSubticks(visualSeparator, cartesianChart);
                UpdateSubticks(visualSeparator.Subticks!, actualScale, s, xc + txco, yc + tyco, UpdateMode.UpdateAndComplete);
            }
            if (LabelsPaint is not null && visualSeparator.Label is null)
            {
                IntializeLabel(visualSeparator, cartesianChart, size, hasRotation, r);
                UpdateLabel(
                    visualSeparator.Label!, xc, yc, TryGetLabelOrLogError(labeler, i - 1d + 1d), hasRotation, r,
                    UpdateMode.UpdateAndComplete);
            }

            #endregion

            if (SeparatorsPaint is not null && visualSeparator.Separator is not null)
            {
                if (ShowSeparatorLines)
                    SeparatorsPaint.AddGeometryToPaintTask(cartesianChart.Canvas, visualSeparator.Separator);
                else
                    SeparatorsPaint.RemoveGeometryFromPainTask(cartesianChart.Canvas, visualSeparator.Separator);
            }

            if (SubseparatorsPaint is not null && visualSeparator.Subseparators is not null)
                if (ShowSeparatorLines)
                    foreach (var subtick in visualSeparator.Subseparators)
                        SubseparatorsPaint.AddGeometryToPaintTask(cartesianChart.Canvas, subtick);
                else
                    foreach (var subtick in visualSeparator.Subseparators)
                        SubseparatorsPaint.RemoveGeometryFromPainTask(cartesianChart.Canvas, subtick);

            if (LabelsPaint is not null && visualSeparator.Label is not null)
                LabelsPaint.AddGeometryToPaintTask(cartesianChart.Canvas, visualSeparator.Label);
            if (TicksPaint is not null && visualSeparator.Tick is not null)
                TicksPaint.AddGeometryToPaintTask(cartesianChart.Canvas, visualSeparator.Tick);
            if (SubticksPaint is not null && visualSeparator.Subticks is not null)
                foreach (var subtick in visualSeparator.Subticks)
                    SubticksPaint.AddGeometryToPaintTask(cartesianChart.Canvas, subtick);

            if (visualSeparator.Separator is not null)
                UpdateSeparator(visualSeparator.Separator, x + sxco, y + syco, lxi, lxj, lyi, lyj, UpdateMode.Update);
            if (visualSeparator.Subseparators is not null)
                UpdateSubseparators(visualSeparator.Subseparators, scale, s, x + sxco, y + tyco, lxi, lxj, lyi, lyj, UpdateMode.Update);
            if (visualSeparator.Tick is not null)
                UpdateTick(visualSeparator.Tick, _tickLength, x + txco, y + tyco, UpdateMode.Update);
            if (visualSeparator.Subticks is not null)
                UpdateSubticks(visualSeparator.Subticks, scale, s, x + txco, y + tyco, UpdateMode.Update);
            if (visualSeparator.Label is not null)
                UpdateLabel(visualSeparator.Label, x, y + tyco, labelContent, hasRotation, r, UpdateMode.Update);

            if (hasActivePaint) _ = measured.Add(visualSeparator);

            if (_stepCount++ > 10000) ThrowInfiniteSeparators();
        }

        foreach (var separatorValueKey in separators.ToArray())
        {
            var separator = separatorValueKey.Value;
            if (measured.Contains(separator)) continue;

            float x, y;
            if (_orientation == AxisOrientation.X)
            {
                x = scale.ToPixels(separator.Value);
                y = yoo;
            }
            else
            {
                x = xoo;
                y = scale.ToPixels(separator.Value);
            }

            if (separator.Separator is not null)
                UpdateSeparator(separator.Separator, x + sxco, y + syco, lxi, lxj, lyi, lyj, UpdateMode.UpdateAndRemove);
            if (separator.Subseparators is not null)
                UpdateSubseparators(
                    separator.Subseparators, scale, s, x + sxco, y + syco, lxi, lxj, lyi, lyj, UpdateMode.UpdateAndRemove);
            if (separator.Tick is not null)
                UpdateTick(separator.Tick, _tickLength, x + txco, y + tyco, UpdateMode.UpdateAndRemove);
            if (separator.Subticks is not null)
                UpdateSubticks(separator.Subticks, scale, s, x + txco, y + tyco, UpdateMode.UpdateAndRemove);
            if (separator.Label is not null)
                UpdateLabel(
                    separator.Label, x, y + tyco, TryGetLabelOrLogError(labeler, separator.Value - 1d + 1d), hasRotation, r,
                    UpdateMode.UpdateAndRemove);

            _ = separators.Remove(separatorValueKey.Key);
        }
    }

    /// <inheritdoc cref="ICartesianAxis{TDrawingContext}.InvalidateCrosshair(Chart{TDrawingContext}, LvcPoint)"/>
    public void InvalidateCrosshair(Chart<TDrawingContext> chart, LvcPoint pointerPosition)
    {
        if (CrosshairPaint is null || chart is not CartesianChart<TDrawingContext> cartesianChart) return;

        var location = chart.DrawMarginLocation;
        var size = chart.DrawMarginSize;

        if (pointerPosition.X < location.X || pointerPosition.X > location.X + size.Width ||
            pointerPosition.Y < location.Y || pointerPosition.Y > location.Y + size.Height)
        {
            return;
        }

        var scale = this.GetNextScaler(cartesianChart);
        var controlSize = cartesianChart.ControlSize;
        var drawLocation = cartesianChart.DrawMarginLocation;
        var drawMarginSize = cartesianChart.DrawMarginSize;
        double labelValue;

        var lyi = drawLocation.Y;
        var lyj = drawLocation.Y + drawMarginSize.Height;
        var lxi = drawLocation.X;
        var lxj = drawLocation.X + drawMarginSize.Width;

        float xoo = 0f, yoo = 0f;

        if (_orientation == AxisOrientation.X)
        {
            yoo = _position == AxisPosition.Start
                 ? controlSize.Height - _yo
                 : _yo;
        }
        else
        {
            xoo = _position == AxisPosition.Start
                ? _xo
                : controlSize.Width - _xo;
        }

        float x, y;
        if (_orientation == AxisOrientation.X)
        {
            float crosshairX;
            if (CrosshairSnapEnabled)
            {
                var axisIndex = Array.IndexOf(cartesianChart.XAxes, this);
                var closestPoint = FindClosestPoint(
                    pointerPosition, cartesianChart,
                    cartesianChart.VisibleSeries
                        .Cast<ICartesianSeries<TDrawingContext>>()
                        .Where(s => s.ScalesXAt == axisIndex));

                var c = closestPoint?.Coordinate;

                crosshairX = scale.ToPixels(c?.SecondaryValue ?? pointerPosition.X);
                labelValue = c?.SecondaryValue ?? scale.ToChartValues(pointerPosition.X);
            }
            else
            {
                crosshairX = pointerPosition.X;
                labelValue = scale.ToChartValues(pointerPosition.X);
            }

            x = crosshairX;
            y = yoo;
        }
        else
        {
            float crosshairY;
            if (CrosshairSnapEnabled)
            {
                var axisIndex = Array.IndexOf(cartesianChart.YAxes, this);
                var closestPoint = FindClosestPoint(
                    pointerPosition, cartesianChart,
                    cartesianChart.VisibleSeries
                        .Cast<ICartesianSeries<TDrawingContext>>()
                        .Where(s => s.ScalesYAt == axisIndex));

                var c = closestPoint?.Coordinate;

                crosshairY = scale.ToPixels(c?.PrimaryValue ?? pointerPosition.Y);
                labelValue = c?.PrimaryValue ?? scale.ToChartValues(pointerPosition.Y);
            }
            else
            {
                crosshairY = pointerPosition.Y;
                labelValue = scale.ToChartValues(pointerPosition.Y);
            }

            x = xoo;
            y = crosshairY;
        }

        if (CrosshairPaint.ZIndex == 0) CrosshairPaint.ZIndex = 1050;
        CrosshairPaint.SetClipRectangle(cartesianChart.Canvas, new LvcRectangle(drawLocation, drawMarginSize));
        cartesianChart.Canvas.AddDrawableTask(CrosshairPaint);

        if (_crosshairLine is null)
        {
            _crosshairLine = new TLineGeometry();
            UpdateSeparator(_crosshairLine, x, y, lxi, lxj, lyi, lyj, UpdateMode.UpdateAndComplete);
        }
        CrosshairPaint.AddGeometryToPaintTask(cartesianChart.Canvas, _crosshairLine);

        if (CrosshairLabelsPaint is not null)
        {
            if (CrosshairLabelsPaint.ZIndex == 0) CrosshairLabelsPaint.ZIndex = 1050;
            if (Orientation == AxisOrientation.X)
            {
                CrosshairLabelsPaint.SetClipRectangle(
                    cartesianChart.Canvas,
                    new LvcRectangle(new LvcPoint(drawLocation.X, 0),
                    new LvcSize(drawMarginSize.Width, controlSize.Height)));
            }
            else
            {
                CrosshairLabelsPaint.SetClipRectangle(
                    cartesianChart.Canvas,
                    new LvcRectangle(new LvcPoint(0, drawLocation.Y),
                    new LvcSize(controlSize.Width, drawMarginSize.Height)));
            }
            cartesianChart.Canvas.AddDrawableTask(CrosshairLabelsPaint);

            _crosshairLabel ??= new TTextGeometry();
            var labeler = GetActualLabeler();

            _crosshairLabel.Text = TryGetLabelOrLogError(labeler, labelValue);
            _crosshairLabel.TextSize = (float)_textSize;
            _crosshairLabel.Background = CrosshairLabelsBackground ?? LvcColor.Empty;
            _crosshairLabel.Padding = CrosshairPadding ?? _padding;
            _crosshairLabel.X = x;
            _crosshairLabel.Y = y;

            var r = (float)_labelsRotation;
            var hasRotation = Math.Abs(r) > 0.01f;
            if (hasRotation) _crosshairLabel.RotateTransform = r;
            CrosshairLabelsPaint.AddGeometryToPaintTask(cartesianChart.Canvas, _crosshairLabel);
        }

        UpdateSeparator(_crosshairLine, x, y, lxi, lxj, lyi, lyj, UpdateMode.Update);

        chart.Canvas.Invalidate();
    }

    /// <inheritdoc cref="ICartesianAxis{TDrawingContext}.ClearCrosshair(Chart{TDrawingContext})"/>
    public void ClearCrosshair(Chart<TDrawingContext> chart)
    {
        if (_crosshairLine is not null)
            CrosshairPaint?.RemoveGeometryFromPainTask(chart.Canvas, _crosshairLine);

        if (_crosshairLabel is not null)
            CrosshairLabelsPaint?.RemoveGeometryFromPainTask(chart.Canvas, _crosshairLabel);
    }

    private IEnumerable<double> EnumerateSeparators(double start, double end, double step)
    {
        if (CustomSeparators is not null)
        {
            foreach (var s in CustomSeparators) yield return s;
            yield break;
        }

        var relativeEnd = end - start;
        for (var i = 0d; i <= relativeEnd; i += step) yield return start + i;
    }

    private static ChartPoint? FindClosestPoint(
        LvcPoint pointerPosition,
        CartesianChart<TDrawingContext> cartesianChart,
        IEnumerable<ICartesianSeries<TDrawingContext>> allSeries)
    {
        ChartPoint? closestPoint = null;
        foreach (var series in allSeries)
        {
            var hitpoints = series.FindHitPoints(cartesianChart, pointerPosition, allSeries.GetTooltipFindingStrategy());
            var hitpoint = hitpoints.FirstOrDefault();
            if (hitpoint == null) continue;

            if (closestPoint is null ||
                hitpoint.DistanceTo(pointerPosition) < closestPoint.DistanceTo(pointerPosition))
            {
                closestPoint = hitpoint;
            }
        }

        return closestPoint;
    }

    /// <inheritdoc cref="IPlane{TDrawingContext}.GetNameLabelSize(Chart{TDrawingContext})"/>
    public LvcSize GetNameLabelSize(Chart<TDrawingContext> chart)
    {
        if (NamePaint is null || string.IsNullOrWhiteSpace(Name)) return new LvcSize(0, 0);

        var textGeometry = new TTextGeometry
        {
            Text = Name ?? string.Empty,
            TextSize = (float)_nameTextSize,
            RotateTransform = Orientation == AxisOrientation.X
                ? 0
                : InLineNamePlacement ? 0 : -90,
            Padding = NamePadding
        };

        return textGeometry.Measure(NamePaint);
    }

    /// <inheritdoc cref="IPlane{TDrawingContext}.GetPossibleSize(Chart{TDrawingContext})"/>
    public virtual LvcSize GetPossibleSize(Chart<TDrawingContext> chart)
    {
        if (_dataBounds is null) throw new Exception("DataBounds not found");
        if (LabelsPaint is null) return new LvcSize(0f, 0f);

        var ts = (float)_textSize;
        var labeler = GetActualLabeler();

        var axisTick = this.GetTick(chart.DrawMarginSize);
        var s = axisTick.Value;

        var max = MaxLimit is null ? _visibleDataBounds.Max : MaxLimit.Value;
        var min = MinLimit is null ? _visibleDataBounds.Min : MinLimit.Value;

        AxisLimit.ValidateLimits(ref min, ref max);

        if (s < _minStep) s = _minStep;
        if (_forceStepToMin) s = _minStep;

        var start = Math.Truncate(min / s) * s;

        var w = 0f;
        var h = 0f;
        var r = (float)LabelsRotation;

        foreach (var i in EnumerateSeparators(start, max, s))
        {
            var textGeometry = new TTextGeometry
            {
                Text = TryGetLabelOrLogError(labeler, i),
                TextSize = ts,
                RotateTransform = r,
                Padding = _padding
            };
            var m = textGeometry.Measure(LabelsPaint);
            if (m.Width > w) w = m.Width;
            if (m.Height > h) h = m.Height;

            if (_stepCount++ > 10000) ThrowInfiniteSeparators();
        }

        return new LvcSize(w, h);
    }

    /// <inheritdoc cref="ICartesianAxis.GetLimits"/>
    public AxisLimit GetLimits()
    {
        var max = MaxLimit is null ? DataBounds.Max : MaxLimit.Value;
        var min = MinLimit is null ? DataBounds.Min : MinLimit.Value;

        AxisLimit.ValidateLimits(ref min, ref max);

        var maxd = DataBounds.Max;
        var mind = DataBounds.Min;
        var minZoomDelta = MinZoomDelta ?? DataBounds.MinDelta * 3;

        foreach (var axis in SharedWith ?? Enumerable.Empty<ICartesianAxis>())
        {
            var maxI = axis.MaxLimit is null ? axis.DataBounds.Max : axis.MaxLimit.Value;
            var minI = axis.MinLimit is null ? axis.DataBounds.Min : axis.MinLimit.Value;
            var maxDI = axis.DataBounds.Max;
            var minDI = axis.DataBounds.Min;
            var minZoomDeltaI = axis.MinZoomDelta ?? axis.DataBounds.MinDelta * 3;

            if (maxI > max) max = maxI;
            if (minI < min) min = minI;
            if (maxDI > maxd) maxd = maxDI;
            if (minDI < mind) mind = minDI;
        }

        return new(min, max, minZoomDelta, mind, maxd);
    }

    /// <inheritdoc cref="ICartesianAxis.SetLimits(double, double)"/>
    public void SetLimits(double min, double max)
    {
        foreach (var axis in SharedWith ?? Enumerable.Empty<ICartesianAxis>())
        {
            axis.MinLimit = min;
            axis.MaxLimit = max;
        }

        MinLimit = min;
        MaxLimit = max;
    }

    /// <inheritdoc cref="ICartesianAxis.Initialize(AxisOrientation)"/>
    void ICartesianAxis.Initialize(AxisOrientation orientation)
    {
        _orientation = orientation;
        _dataBounds = new Bounds();
        _visibleDataBounds = new Bounds();
        _animatableBounds ??= new();
        _possibleMaxLabelsSize = null;
        Initialized?.Invoke(this);
    }

    /// <summary>
    /// Deletes the specified chart.
    /// </summary>
    /// <param name="chart">The chart.</param>
    /// <returns></returns>
    public virtual void Delete(Chart<TDrawingContext> chart)
    {
        foreach (var paint in GetPaintTasks())
        {
            if (paint is null) continue;

            chart.Canvas.RemovePaintTask(paint);
            paint.ClearGeometriesFromPaintTask(chart.Canvas);
        }

        _ = activeSeparators.Remove(chart);
    }

    /// <inheritdoc cref="IChartElement{TDrawingContext}.RemoveFromUI(Chart{TDrawingContext})"/>
    public override void RemoveFromUI(Chart<TDrawingContext> chart)
    {
        base.RemoveFromUI(chart);
        _animatableBounds = new();
        _ = activeSeparators.Remove(chart);
    }

    /// <summary>
    /// Called when [paint changed].
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns></returns>
    protected override void OnPaintChanged(string? propertyName)
    {
        base.OnPaintChanged(propertyName);
        OnPropertyChanged(propertyName);
    }

    /// <summary>
    /// Gets the paint tasks.
    /// </summary>
    /// <returns></returns>
    internal override IPaint<TDrawingContext>?[] GetPaintTasks()
    {
        return new[] { _separatorsPaint, _labelsPaint, _namePaint, _zeroPaint, _ticksPaint, _subticksPaint, _subseparatorsPaint };
    }

    private Func<double, string> GetActualLabeler()
    {
        var labeler = Labeler;

        if (Labels is not null)
        {
            labeler = Labelers.BuildNamedLabeler(Labels);
            _minStep = 1;
        }

        return labeler;
    }

    private LvcSize GetPossibleMaxLabelSize()
    {
        if (LabelsPaint is null) return new LvcSize();

        var labeler = GetActualLabeler();

        var max = MaxLimit is null ? _visibleDataBounds.Max : MaxLimit.Value;
        var min = MinLimit is null ? _visibleDataBounds.Min : MinLimit.Value;

        AxisLimit.ValidateLimits(ref min, ref max);

        const double testSeparators = 25;
        var s = (max - min) / testSeparators;
        if (s == 0) s = 1;
        if (s < _minStep) s = _minStep;
        if (_forceStepToMin) s = _minStep;

        var maxLabelSize = new LvcSize();

        if (max - min == 0) return maxLabelSize;

        foreach (var i in EnumerateSeparators(min, max, s))
        {
            var textGeometry = new TTextGeometry
            {
                Text = labeler(i),
                TextSize = (float)_textSize,
                RotateTransform = (float)LabelsRotation,
                Padding = _padding
            };

            var m = textGeometry.Measure(LabelsPaint);

            maxLabelSize = new LvcSize(
                maxLabelSize.Width > m.Width ? maxLabelSize.Width : m.Width,
                maxLabelSize.Height > m.Height ? maxLabelSize.Height : m.Height);

            if (_stepCount++ > 10000) ThrowInfiniteSeparators();
        }

        return maxLabelSize;
    }

    private void DrawName(
        CartesianChart<TDrawingContext> cartesianChart,
        float size,
        float lxi,
        float lxj,
        float lyi,
        float lyj)
    {
        var isNew = false;

        if (_nameGeometry is null)
        {
            _nameGeometry = new TTextGeometry
            {
                TextSize = size,
                HorizontalAlign = Align.Middle,
                VerticalAlign = Align.Middle
            };

            _nameGeometry.Animate(EasingFunction ?? cartesianChart.EasingFunction, AnimationsSpeed ?? cartesianChart.AnimationsSpeed);
            isNew = true;
        }

        _nameGeometry.Padding = NamePadding;
        _nameGeometry.Text = Name ?? string.Empty;
        _nameGeometry.TextSize = (float)_nameTextSize;

        if (_orientation == AxisOrientation.X)
        {
            if (InLineNamePlacement)
            {
                _nameGeometry.X = _nameDesiredSize.X + _nameDesiredSize.Width * 0.5f;
                _nameGeometry.Y = _nameDesiredSize.Y + _nameDesiredSize.Height * 0.5f;
            }
            else
            {
                _nameGeometry.X = (lxi + lxj) * 0.5f;
                _nameGeometry.Y = _nameDesiredSize.Y + _nameDesiredSize.Height * 0.5f;
            }
        }
        else
        {
            if (InLineNamePlacement)
            {
                _nameGeometry.X = _nameDesiredSize.X + _nameDesiredSize.Width * 0.5f;
                _nameGeometry.Y = _nameDesiredSize.Height * 0.5f;
            }
            else
            {
                _nameGeometry.RotateTransform = -90;
                _nameGeometry.X = _nameDesiredSize.X + _nameDesiredSize.Width * 0.5f;
                _nameGeometry.Y = (lyi + lyj) * 0.5f;
            }
        }

        if (isNew) _nameGeometry.CompleteTransition(null);
    }

    private void InitializeSeparator(
        AxisVisualSeprator<TDrawingContext> visualSeparator, CartesianChart<TDrawingContext> cartesianChart, TLineGeometry? separatorGeometry = null)
    {
        TLineGeometry lineGeometry;

        if (separatorGeometry is not null)
        {
            lineGeometry = separatorGeometry;
        }
        else
        {
            lineGeometry = new TLineGeometry();
            visualSeparator.Separator = lineGeometry;
        }

        visualSeparator.Separator = lineGeometry;
        InitializeLine(lineGeometry, cartesianChart);
    }

    private void InitializeSubseparators(
        AxisVisualSeprator<TDrawingContext> visualSeparator, CartesianChart<TDrawingContext> cartesianChart)
    {
        visualSeparator.Subseparators = new TLineGeometry[_subseparatorsCount];

        for (var j = 0; j < _subseparatorsCount; j++)
        {
            var subSeparator = new TLineGeometry();
            visualSeparator.Subseparators[j] = subSeparator;
            InitializeTick(visualSeparator, cartesianChart, subSeparator);
        }
    }

    private void InitializeLine(ILineGeometry<TDrawingContext> lineGeometry, CartesianChart<TDrawingContext> cartesianChart)
    {
        lineGeometry.Animate(EasingFunction ?? cartesianChart.EasingFunction, AnimationsSpeed ?? cartesianChart.AnimationsSpeed);
    }

    private void InitializeTick(
        AxisVisualSeprator<TDrawingContext> visualSeparator, CartesianChart<TDrawingContext> cartesianChart, TLineGeometry? subTickGeometry = null)
    {
        TLineGeometry tickGeometry;

        if (subTickGeometry is not null)
        {
            tickGeometry = subTickGeometry;
        }
        else
        {
            tickGeometry = new TLineGeometry();
            visualSeparator.Tick = tickGeometry;
        }

        tickGeometry.Animate(EasingFunction ?? cartesianChart.EasingFunction, AnimationsSpeed ?? cartesianChart.AnimationsSpeed);
    }

    private void InitializeSubticks(
        AxisVisualSeprator<TDrawingContext> visualSeparator, CartesianChart<TDrawingContext> cartesianChart)
    {
        visualSeparator.Subticks = new TLineGeometry[_subseparatorsCount];

        for (var j = 0; j < _subseparatorsCount; j++)
        {
            var subTick = new TLineGeometry();
            visualSeparator.Subticks[j] = subTick;
            InitializeTick(visualSeparator, cartesianChart, subTick);
        }
    }

    private void IntializeLabel(
        AxisVisualSeprator<TDrawingContext> visualSeparator,
        CartesianChart<TDrawingContext> cartesianChart,
        float size,
        bool hasRotation,
        float r)
    {
        var textGeometry = new TTextGeometry { TextSize = size };
        visualSeparator.Label = textGeometry;
        if (hasRotation) textGeometry.RotateTransform = r;

        textGeometry.Animate(EasingFunction ?? cartesianChart.EasingFunction, AnimationsSpeed ?? cartesianChart.AnimationsSpeed);
    }

    private void UpdateSeparator(
        ILineGeometry<TDrawingContext> line,
        float x,
        float y,
        float lxi,
        float lxj,
        float lyi,
        float lyj,
        UpdateMode mode)
    {
        if (_orientation == AxisOrientation.X)
        {
            line.X = x;
            line.X1 = x;
            line.Y = lyi;
            line.Y1 = lyj;
        }
        else
        {
            line.X = lxi;
            line.X1 = lxj;
            line.Y = y;
            line.Y1 = y;
        }

        SetUpdateMode(line, mode);
    }

    private void UpdateTick(
        ILineGeometry<TDrawingContext> tick, float length, float x, float y, UpdateMode mode)
    {
        if (_orientation == AxisOrientation.X)
        {
            var lyi = y + _size.Height * 0.5f;
            var lyj = y - _size.Height * 0.5f;
            tick.X = x;
            tick.X1 = x;
            tick.Y = _position == AxisPosition.Start ? lyj : lyi - length;
            tick.Y1 = _position == AxisPosition.Start ? lyj + length : lyi;
        }
        else
        {
            var lxi = x + _size.Width * 0.5f;
            var lxj = x - _size.Width * 0.5f;
            tick.X = _position == AxisPosition.Start ? lxi : lxj + length;
            tick.X1 = _position == AxisPosition.Start ? lxi - length : lxj;
            tick.Y = y;
            tick.Y1 = y;
        }

        SetUpdateMode(tick, mode);
    }

    private void UpdateSubseparators(
        ILineGeometry<TDrawingContext>[] subseparators, Scaler scale, double s, float x, float y, float lxi, float lxj, float lyi, float lyj, UpdateMode mode)
    {
        for (var j = 0; j < subseparators.Length; j++)
        {
            var subseparator = subseparators[j];
            var kl = (j + 1) / (double)(_subseparatorsCount + 1);

            if (_logBase is not null) kl = Math.Log(kl, _logBase.Value);

            float xs = 0f, ys = 0f;
            if (_orientation == AxisOrientation.X)
            {
                xs = scale.MeasureInPixels(s * kl);
            }
            else
            {
                ys = scale.MeasureInPixels(s * kl);
            }

            UpdateSeparator(subseparator, x + xs, y + ys, lxi, lxj, lyi, lyj, mode);
        }
    }

    private void UpdateSubticks(
        ILineGeometry<TDrawingContext>[] subticks, Scaler scale, double s, float x, float y, UpdateMode mode)
    {
        for (var j = 0; j < subticks.Length; j++)
        {
            var subtick = subticks[j];

            var k = 0.5f;
            var kl = (j + 1) / (double)(_subseparatorsCount + 1);
            if (Math.Abs(kl - 0.5f) < 0.01) k += 0.25f;

            float xs = 0f, ys = 0f;
            if (_orientation == AxisOrientation.X)
            {
                xs = scale.MeasureInPixels(s * kl);
            }
            else
            {
                ys = scale.MeasureInPixels(s * kl);
            }

            UpdateTick(subtick, _tickLength * k, x + xs, y + ys, mode);
        }
    }

    private void UpdateLabel(
        ILabelGeometry<TDrawingContext> label,
        float x,
        float y,
        string text,
        bool hasRotation,
        float r,
        UpdateMode mode)
    {
        var actualRotatation = r;
        const double toRadians = Math.PI / 180;

        if (_orientation == AxisOrientation.Y)
        {
            actualRotatation %= 180;
            if (actualRotatation < 0) actualRotatation += 360;
            if (actualRotatation is > 90 and < 180) actualRotatation += 180;
            if (actualRotatation is > 180 and < 270) actualRotatation += 180;

            var actualAlignment = _labelsAlignment == null
              ? (_position == AxisPosition.Start ? Align.End : Align.Start)
              : _labelsAlignment.Value;

            if (actualAlignment == Align.Start)
            {
                if (hasRotation && _labelsPaint is not null)
                {
                    var notRotatedSize =
                        new TTextGeometry { TextSize = (float)_textSize, Padding = _padding, Text = text }
                        .Measure(_labelsPaint);

                    var rhx = Math.Cos((90 - actualRotatation) * toRadians) * notRotatedSize.Height;
                    x += (float)Math.Abs(rhx * 0.5f);
                }

                x -= _labelsDesiredSize.Width * 0.5f;
                label.HorizontalAlign = Align.Start;
            }
            else
            {
                if (hasRotation && _labelsPaint is not null)
                {
                    var notRotatedSize =
                        new TTextGeometry { TextSize = (float)_textSize, Padding = _padding, Text = text }
                        .Measure(_labelsPaint);

                    var rhx = Math.Cos((90 - actualRotatation) * toRadians) * notRotatedSize.Height;
                    x -= (float)Math.Abs(rhx * 0.5f);
                }

                x += _labelsDesiredSize.Width * 0.5f;
                label.HorizontalAlign = Align.End;
            }
        }

        if (_orientation == AxisOrientation.X)
        {
            actualRotatation %= 180;
            if (actualRotatation < 0) actualRotatation += 180;
            if (actualRotatation >= 90) actualRotatation -= 180;

            var actualAlignment = _labelsAlignment == null
              ? (_position == AxisPosition.Start ? Align.Start : Align.End)
              : _labelsAlignment.Value;

            if (actualAlignment == Align.Start)
            {
                if (hasRotation && _labelsPaint is not null)
                {
                    var notRotatedSize =
                        new TTextGeometry { TextSize = (float)_textSize, Padding = _padding, Text = text }
                        .Measure(_labelsPaint);

                    var rhx = Math.Sin((90 - actualRotatation) * toRadians) * notRotatedSize.Height;
                    y += (float)Math.Abs(rhx * 0.5f);
                }

                if (hasRotation)
                {
                    y -= _labelsDesiredSize.Height * 0.5f;
                    label.HorizontalAlign = actualRotatation < 0
                        ? Align.End
                        : Align.Start;
                }
                else
                {
                    label.HorizontalAlign = Align.Middle;
                }
            }
            else
            {
                if (hasRotation && _labelsPaint is not null)
                {
                    var notRotatedSize =
                        new TTextGeometry { TextSize = (float)_textSize, Padding = _padding, Text = text }
                        .Measure(_labelsPaint);

                    var rhx = Math.Sin((90 - actualRotatation) * toRadians) * notRotatedSize.Height;
                    y -= (float)Math.Abs(rhx * 0.5f);
                }

                if (hasRotation)
                {
                    y += _labelsDesiredSize.Height * 0.5f;
                    label.HorizontalAlign = actualRotatation < 0
                        ? Align.Start
                        : Align.End;
                }
                else
                {
                    label.HorizontalAlign = Align.Middle;
                }
            }
        }

        label.Text = text;
        label.Padding = _padding;
        label.X = x;
        label.Y = y;

        if (hasRotation) label.RotateTransform = actualRotatation;

        SetUpdateMode(label, mode);
    }

    private void SetUpdateMode(IGeometry<TDrawingContext> geometry, UpdateMode mode)
    {
        switch (mode)
        {
            case CoreAxis<TDrawingContext, TTextGeometry, TLineGeometry>.UpdateMode.UpdateAndComplete:
                if (_animatableBounds.HasPreviousState) geometry.Opacity = 0;
                geometry.CompleteTransition(null);
                break;
            case CoreAxis<TDrawingContext, TTextGeometry, TLineGeometry>.UpdateMode.UpdateAndRemove:
                geometry.Opacity = 0;
                geometry.RemoveOnCompleted = true;
                break;
            case CoreAxis<TDrawingContext, TTextGeometry, TLineGeometry>.UpdateMode.Update:
            default:
                geometry.Opacity = 1;
                break;
        }
    }

    private string TryGetLabelOrLogError(Func<double, string> labeler, double value)
    {
        try
        {
            return labeler(value);
        }
        catch (Exception e)
        {
#if DEBUG
            Trace.WriteLine($"[Error] LiveCharts was not able to get a label from axis {_orientation} with value {value}. {e.Message}");
#endif
            return string.Empty;
        }
    }

    private void ThrowInfiniteSeparators()
    {
        throw new Exception(
            $"The {_orientation} axis has an excessive number of separators. " +
            $"If you set the step manually, ensure the number of separators is less than 10,000. " +
            $"This could also be caused because you are zooming too deep, " +
            $"try to set a limit to the current chart zoom using the Axis.{nameof(MinZoomDelta)} property. " +
            $"For more info see: https://github.com/beto-rodriguez/LiveCharts2/issues/1076.");
    }

    private enum UpdateMode
    {
        Update,
        UpdateAndComplete,
        UpdateAndRemove
    }
}
