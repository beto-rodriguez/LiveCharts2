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
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
public abstract class Axis<TDrawingContext, TTextGeometry, TLineGeometry>
    : ChartElement<TDrawingContext>, ICartesianAxis<TDrawingContext>, IPlane<TDrawingContext>
        where TDrawingContext : DrawingContext
        where TTextGeometry : ILabelGeometry<TDrawingContext>, new()
        where TLineGeometry : class, ILineGeometry<TDrawingContext>, new()
{
    #region fields

    /// <summary>
    /// The active separators
    /// </summary>
    protected readonly Dictionary<IChart, Dictionary<string, AxisVisualSeprator<TDrawingContext>>> activeSeparators = new();

    internal float _xo = 0f, _yo = 0f;
    internal LvcSize _size;
    internal AxisOrientation _orientation;
    internal AnimatableAxisBounds _animatableBounds = new();
    internal Bounds _dataBounds = new();
    internal Bounds _visibleDataBounds = new();

    private double _minStep = 0;
    private double _labelsRotation;
    private LvcRectangle _labelsDesiredSize = new(), _nameDesiredSize = new();
    private TTextGeometry? _nameGeometry;
    private AxisPosition _position = AxisPosition.Start;
    private Func<double, string> _labeler = Labelers.Default;
    private Padding _padding = Padding.Default;
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
    private bool _showSeparatorLines = true;
    private bool _isVisible = true;
    private bool _isInverted;
    private bool _forceStepToMin;
    private readonly float _tickLength = 8f;
    private readonly int _subSections = 3;

    #endregion

    #region properties

    float ICartesianAxis.Xo { get => _xo; set => _xo = value; }
    float ICartesianAxis.Yo { get => _yo; set => _yo = value; }
    LvcSize ICartesianAxis.Size { get => _size; set => _size = value; }
    LvcRectangle ICartesianAxis.LabelsDesiredSize { get => _labelsDesiredSize; set => _labelsDesiredSize = value; }
    LvcRectangle ICartesianAxis.NameDesiredSize { get => _nameDesiredSize; set => _nameDesiredSize = value; }

    /// <inheritdoc cref="IPlane.DataBounds"/>
    public Bounds DataBounds => _dataBounds;

    /// <inheritdoc cref="IPlane.VisibleDataBounds"/>
    public Bounds VisibleDataBounds => _visibleDataBounds;

    AnimatableAxisBounds IPlane.ActualBounds => _animatableBounds;

    /// <inheritdoc cref="IPlane.Name"/>
    public string? Name { get; set; } = null;

    /// <inheritdoc cref="IPlane.NameTextSize"/>
    public double NameTextSize { get => _nameTextSize; set { _nameTextSize = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IPlane.NamePadding"/>
    public Padding NamePadding { get => _namePadding; set { _namePadding = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="ICartesianAxis.Orientation"/>
    public AxisOrientation Orientation => _orientation;

    /// <inheritdoc cref="ICartesianAxis.Padding"/>
    public Padding Padding { get => _padding; set { _padding = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IPlane.Labeler"/>
    public Func<double, string> Labeler { get => _labeler; set { _labeler = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IPlane.MinStep"/>
    public double MinStep { get => _minStep; set { _minStep = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IPlane.ForceStepToMin"/>
    public bool ForceStepToMin { get => _forceStepToMin; set { _forceStepToMin = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IPlane.MinLimit"/>
    public double? MinLimit { get => _minLimit; set { _minLimit = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IPlane.MaxLimit"/>
    public double? MaxLimit { get => _maxLimit; set { _maxLimit = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IPlane.UnitWidth"/>
    public double UnitWidth { get => _unitWidth; set { _unitWidth = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="ICartesianAxis.Position"/>
    public AxisPosition Position { get => _position; set { _position = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IPlane.LabelsRotation"/>
    public double LabelsRotation { get => _labelsRotation; set { _labelsRotation = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IPlane.TextSize"/>
    public double TextSize { get => _textSize; set { _textSize = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IPlane.Labels"/>
    public IList<string>? Labels { get; set; }

    /// <inheritdoc cref="IPlane.ShowSeparatorLines"/>
    public bool ShowSeparatorLines { get => _showSeparatorLines; set { _showSeparatorLines = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="ICartesianAxis{TDrawingContext}.SubseparatorsPaint"/>
    public IPaint<TDrawingContext>? SubseparatorsPaint { get => _subseparatorsPaint; set { _subseparatorsPaint = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="ICartesianAxis{TDrawingContext}.DrawTicksPath"/>
    public bool DrawTicksPath { get => _drawTicksPath; set { _drawTicksPath = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="ICartesianAxis{TDrawingContext}.TicksPaint"/>
    public IPaint<TDrawingContext>? TicksPaint { get => _ticksPaint; set { _ticksPaint = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="ICartesianAxis{TDrawingContext}.SubticksPaint"/>
    public IPaint<TDrawingContext>? SubticksPaint { get => _subticksPaint; set { _subticksPaint = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="ICartesianAxis{TDrawingContext}.ZeroPaint"/>
    public IPaint<TDrawingContext>? ZeroPaint { get => _zeroPaint; set { _zeroPaint = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IPlane.IsVisible"/>
    public bool IsVisible { get => _isVisible; set { _isVisible = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IPlane.IsInverted"/>
    public bool IsInverted { get => _isInverted; set { _isInverted = value; OnPropertyChanged(); } }

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
        set => SetPaintProperty(ref _separatorsPaint, value);
    }

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

    /// <inheritdoc cref="IStopNPC.IsNotifyingChanges"/>
    bool IStopNPC.IsNotifyingChanges { get; set; }

    #endregion

    /// <inheritdoc cref="ICartesianAxis.Initialized"/>
    public event Action<ICartesianAxis>? Initialized;

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    /// <returns></returns>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc cref="ChartElement{TDrawingContext}.Measure(Chart{TDrawingContext})"/>
    public override void Measure(Chart<TDrawingContext> chart)
    {
        var cartesianChart = (CartesianChart<TDrawingContext>)chart;

        var controlSize = cartesianChart.ControlSize;
        var drawLocation = cartesianChart.DrawMarginLocation;
        var drawMarginSize = cartesianChart.DrawMarginSize;

        var max = MaxLimit is null ? _visibleDataBounds.Max : MaxLimit.Value;
        var min = MinLimit is null ? _visibleDataBounds.Min : MinLimit.Value;

        _animatableBounds.MaxVisibleBound = max;
        _animatableBounds.MinVisibleBound = min;

        if (!_animatableBounds.HasPreviousState)
        {
            _ = _animatableBounds
                .TransitionateProperties(null)
                .WithAnimation(animation =>
                         animation
                             .WithDuration(AnimationsSpeed ?? cartesianChart.AnimationsSpeed)
                             .WithEasingFunction(EasingFunction ?? cartesianChart.EasingFunction))
                .CompleteCurrentTransitions();

            _ = cartesianChart.Canvas.Trackers.Add(_animatableBounds);
        }

        var scale = this.GetNextScaler(cartesianChart);
        var actualScale = this.GetActualScalerScaler(cartesianChart) ?? scale;

        var axisTick = this.GetTick(drawMarginSize, null, GetPossibleMaxLabelSize());

        var labeler = Labeler;
        if (Labels is not null)
        {
            labeler = Labelers.BuildNamedLabeler(Labels).Function;
            _minStep = 1;
        }

        var s = axisTick.Value;
        if (s < _minStep) s = _minStep;
        if (_forceStepToMin) s = _minStep;

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
        if (SubseparatorsPaint is not null)
        {
            if (SubseparatorsPaint.ZIndex == 0) SubseparatorsPaint.ZIndex = -1;
            SubseparatorsPaint.SetClipRectangle(cartesianChart.Canvas, new LvcRectangle(drawLocation, drawMarginSize));
            cartesianChart.Canvas.AddDrawableTask(SubseparatorsPaint);
        }
        if (SeparatorsPaint is not null)
        {
            if (SeparatorsPaint.ZIndex == 0) SeparatorsPaint.ZIndex = -1;
            SeparatorsPaint.SetClipRectangle(cartesianChart.Canvas, new LvcRectangle(drawLocation, drawMarginSize));
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

        var start = Math.Truncate(min / s) * s;
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
                TicksPaint.AddGeometryToPaintTask(cartesianChart.Canvas, _ticksPath);
            }

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

        for (var i = start - s; i <= max + s; i += s)
        {
            var separatorKey = labeler(i - 1d + 1d);
            var labelContent = i < min || i > max ? string.Empty : separatorKey;

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

            float xc = 0, yc = 0;
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

                if (SeparatorsPaint is not null && ShowSeparatorLines)
                {
                    InitializeSeparator(visualSeparator, cartesianChart);
                    UpdateSeparator(visualSeparator.Separator!, xc, yc, lxi, lxj, lyi, lyj, UpdateMode.UpdateAndComplete);
                }
                if (SubseparatorsPaint is not null)
                {
                    InitializeSubseparators(visualSeparator, cartesianChart);
                    UpdateSubseparators(visualSeparator.Subseparators!, actualScale, s, xc, yc, lxi, lxj, lyi, lyj, UpdateMode.UpdateAndComplete);
                }
                if (TicksPaint is not null)
                {
                    InitializeTick(visualSeparator, cartesianChart);
                    UpdateTick(visualSeparator.Tick!, _tickLength, xc, yc, UpdateMode.UpdateAndComplete);
                }
                if (SubticksPaint is not null && _subSections > 0)
                {
                    InitializeSubticks(visualSeparator, cartesianChart);
                    UpdateSubticks(visualSeparator.Subticks!, actualScale, s, xc, yc, UpdateMode.UpdateAndComplete);
                }
                if (LabelsPaint is not null)
                {
                    IntializeLabel(visualSeparator, cartesianChart, size, hasRotation, r);
                    UpdateLabel(visualSeparator.Label!, xc, yc, labeler(i - 1d + 1d), hasRotation, r, UpdateMode.UpdateAndComplete);
                }

                separators.Add(separatorKey, visualSeparator);
            }

            if (SeparatorsPaint is not null && visualSeparator.Separator is not null)
            {
                if (ShowSeparatorLines)
                    SeparatorsPaint.AddGeometryToPaintTask(cartesianChart.Canvas, visualSeparator.Separator);
                else
                    SeparatorsPaint.RemoveGeometryFromPainTask(cartesianChart.Canvas, visualSeparator.Separator);
            }
            if (SubseparatorsPaint is not null && visualSeparator.Subseparators is not null)
                foreach (var subtick in visualSeparator.Subseparators) SubseparatorsPaint.AddGeometryToPaintTask(cartesianChart.Canvas, subtick);
            if (LabelsPaint is not null && visualSeparator.Label is not null)
                LabelsPaint.AddGeometryToPaintTask(cartesianChart.Canvas, visualSeparator.Label);
            if (TicksPaint is not null && visualSeparator.Tick is not null)
                TicksPaint.AddGeometryToPaintTask(cartesianChart.Canvas, visualSeparator.Tick);
            if (SubticksPaint is not null && visualSeparator.Subticks is not null)
                foreach (var subtick in visualSeparator.Subticks) SubticksPaint.AddGeometryToPaintTask(cartesianChart.Canvas, subtick);

            if (visualSeparator.Separator is not null) UpdateSeparator(visualSeparator.Separator, x, y, lxi, lxj, lyi, lyj, UpdateMode.Update);
            if (visualSeparator.Subseparators is not null) UpdateSubseparators(visualSeparator.Subseparators, scale, s, x, y, lxi, lxj, lyi, lyj, UpdateMode.Update);
            if (visualSeparator.Tick is not null) UpdateTick(visualSeparator.Tick, _tickLength, x, y, UpdateMode.Update);
            if (visualSeparator.Subticks is not null) UpdateSubticks(visualSeparator.Subticks, scale, s, x, y, UpdateMode.Update);
            if (visualSeparator.Label is not null) UpdateLabel(visualSeparator.Label, x, y, labelContent, hasRotation, r, UpdateMode.Update);

            if (hasActivePaint) _ = measured.Add(visualSeparator);
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

            if (separator.Separator is not null) UpdateSeparator(separator.Separator, x, y, lxi, lxj, lyi, lyj, UpdateMode.UpdateAndRemove);
            if (separator.Subseparators is not null) UpdateSubseparators(separator.Subseparators, scale, s, x, y, lxi, lxj, lyi, lyj, UpdateMode.UpdateAndRemove);
            if (separator.Tick is not null) UpdateTick(separator.Tick, _tickLength, x, y, UpdateMode.UpdateAndRemove);
            if (separator.Subticks is not null) UpdateSubticks(separator.Subticks, scale, s, x, y, UpdateMode.UpdateAndRemove);
            if (separator.Label is not null) UpdateLabel(separator.Label, x, y, labeler(separator.Value - 1d + 1d), hasRotation, r, UpdateMode.UpdateAndRemove);

            _ = separators.Remove(separatorValueKey.Key);
        }
    }

    /// <inheritdoc cref="IPlane{TDrawingContext}.GetNameLabelSize(Chart{TDrawingContext})"/>
    public LvcSize GetNameLabelSize(Chart<TDrawingContext> chart)
    {
        if (NamePaint is null || string.IsNullOrWhiteSpace(Name)) return new LvcSize(0, 0);

        var textGeometry = new TTextGeometry
        {
            Text = Name ?? string.Empty,
            TextSize = (float)NameTextSize,
            RotateTransform = Orientation == AxisOrientation.X ? 0 : -90,
            Padding = NamePadding
        };

        return textGeometry.Measure(NamePaint);
    }

    /// <inheritdoc cref="IPlane{TDrawingContext}.GetPossibleSize(Chart{TDrawingContext})"/>
    public virtual LvcSize GetPossibleSize(Chart<TDrawingContext> chart)
    {
        if (_dataBounds is null) throw new Exception("DataBounds not found");
        if (LabelsPaint is null) return new LvcSize(0f, 0f);

        var ts = (float)TextSize;
        var labeler = Labeler;

        if (Labels is not null)
        {
            labeler = Labelers.BuildNamedLabeler(Labels).Function;
            _minStep = 1;
        }

        var axisTick = this.GetTick(chart.DrawMarginSize);

        var s = axisTick.Value;

        if (s == 0) s = 1;
        if (s < _minStep) s = _minStep;
        if (_forceStepToMin) s = _minStep;

        var max = MaxLimit is null ? _visibleDataBounds.Max : MaxLimit.Value;
        var min = MinLimit is null ? _visibleDataBounds.Min : MinLimit.Value;

        var start = Math.Truncate(min / s) * s;

        var w = 0f;
        var h = 0f;
        var r = (float)LabelsRotation;

        for (var i = start; i <= max; i += s)
        {
            var textGeometry = new TTextGeometry
            {
                Text = labeler(i),
                TextSize = ts,
                RotateTransform = r,
                Padding = _padding
            };
            var m = textGeometry.Measure(LabelsPaint);
            if (m.Width > w) w = m.Width;
            if (m.Height > h) h = m.Height;
        }

        return new LvcSize(w, h);
    }

    /// <inheritdoc cref="ICartesianAxis.Initialize(AxisOrientation)"/>
    void ICartesianAxis.Initialize(AxisOrientation orientation)
    {
        _orientation = orientation;
        _dataBounds = new Bounds();
        _visibleDataBounds = new Bounds();
        _animatableBounds ??= new();
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
        _animatableBounds = null!;
        _ = activeSeparators.Remove(chart);
    }

    /// <summary>
    /// Called when a property changes.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns></returns>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        if (!((ICartesianAxis)this).IsNotifyingChanges) return;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

    private LvcSize GetPossibleMaxLabelSize()
    {
        if (LabelsPaint is null) return new LvcSize();

        var labeler = Labeler;

        if (Labels is not null)
        {
            labeler = Labelers.BuildNamedLabeler(Labels).Function;
            _minStep = 1;
        }

        var max = MaxLimit is null ? _visibleDataBounds.Max : MaxLimit.Value;
        var min = MinLimit is null ? _visibleDataBounds.Min : MinLimit.Value;
        var s = (max - min) / 20d;

        var maxLabelSize = new LvcSize();
        for (var i = min; i <= max; i += s)
        {
            var textGeometry = new TTextGeometry
            {
                Text = labeler(i),
                TextSize = (float)TextSize,
                RotateTransform = (float)LabelsRotation,
                Padding = _padding
            };

            var m = textGeometry.Measure(LabelsPaint);

            maxLabelSize = new LvcSize(
                maxLabelSize.Width > m.Width ? maxLabelSize.Width : m.Width,
                maxLabelSize.Height > m.Height ? maxLabelSize.Height : m.Height);
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

            _ = _nameGeometry
                 .TransitionateProperties(
                         nameof(_nameGeometry.X),
                         nameof(_nameGeometry.Y))
                 .WithAnimation(animation =>
                     animation
                         .WithDuration(AnimationsSpeed ?? cartesianChart.AnimationsSpeed)
                         .WithEasingFunction(EasingFunction ?? cartesianChart.EasingFunction));

            isNew = true;
        }

        _nameGeometry.Padding = NamePadding;
        _nameGeometry.Text = Name ?? string.Empty;
        _nameGeometry.TextSize = (float)NameTextSize;

        if (_orientation == AxisOrientation.X)
        {
            _nameGeometry.X = (lxi + lxj) * 0.5f;
            _nameGeometry.Y = _nameDesiredSize.Y + _nameDesiredSize.Height * 0.5f;
        }
        else
        {
            _nameGeometry.RotateTransform = -90;
            _nameGeometry.X = _nameDesiredSize.X + _nameDesiredSize.Width * 0.5f;
            _nameGeometry.Y = (lyi + lyj) * 0.5f;
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
        visualSeparator.Subseparators = new TLineGeometry[_subSections];

        for (var j = 0; j < _subSections; j++)
        {
            var subSeparator = new TLineGeometry();
            visualSeparator.Subseparators[j] = subSeparator;
            InitializeTick(visualSeparator, cartesianChart, subSeparator);
        }
    }

    private void InitializeLine(ILineGeometry<TDrawingContext> lineGeometry, CartesianChart<TDrawingContext> cartesianChart)
    {
        _ = lineGeometry
            .TransitionateProperties(
                nameof(lineGeometry.X), nameof(lineGeometry.X1),
                nameof(lineGeometry.Y), nameof(lineGeometry.Y1),
                nameof(lineGeometry.Opacity))
            .WithAnimation(animation =>
                animation
                    .WithDuration(AnimationsSpeed ?? cartesianChart.AnimationsSpeed)
                    .WithEasingFunction(EasingFunction ?? cartesianChart.EasingFunction));
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

        _ = tickGeometry
            .TransitionateProperties(
                nameof(tickGeometry.X), nameof(tickGeometry.X1),
                nameof(tickGeometry.Y), nameof(tickGeometry.Y1),
                nameof(tickGeometry.Opacity))
            .WithAnimation(animation =>
                animation
                    .WithDuration(AnimationsSpeed ?? cartesianChart.AnimationsSpeed)
                    .WithEasingFunction(EasingFunction ?? cartesianChart.EasingFunction));
    }

    private void InitializeSubticks(
        AxisVisualSeprator<TDrawingContext> visualSeparator, CartesianChart<TDrawingContext> cartesianChart)
    {
        visualSeparator.Subticks = new TLineGeometry[_subSections];

        for (var j = 0; j < _subSections; j++)
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

        _ = textGeometry
            .TransitionateProperties(
                nameof(textGeometry.X),
                nameof(textGeometry.Y),
                nameof(textGeometry.Opacity))
            .WithAnimation(animation =>
                animation
                    .WithDuration(AnimationsSpeed ?? cartesianChart.AnimationsSpeed)
                    .WithEasingFunction(EasingFunction ?? cartesianChart.EasingFunction));
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
            var kl = (j + 1) / (double)(_subSections + 1);

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
            var kl = (j + 1) / (double)(_subSections + 1);
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
        label.Text = text;
        label.Padding = _padding;
        label.X = x;
        label.Y = y;
        if (hasRotation) label.RotateTransform = r;

        SetUpdateMode(label, mode);
    }

    private void SetUpdateMode(IGeometry<TDrawingContext> geometry, UpdateMode mode)
    {
        switch (mode)
        {
            case Axis<TDrawingContext, TTextGeometry, TLineGeometry>.UpdateMode.UpdateAndComplete:
                if (_animatableBounds.HasPreviousState) geometry.Opacity = 0;
                geometry.CompleteTransition(null);
                break;
            case Axis<TDrawingContext, TTextGeometry, TLineGeometry>.UpdateMode.UpdateAndRemove:
                geometry.Opacity = 0;
                geometry.RemoveOnCompleted = true;
                break;
            case Axis<TDrawingContext, TTextGeometry, TLineGeometry>.UpdateMode.Update:
            default:
                geometry.Opacity = 1;
                break;
        }
    }

    private enum UpdateMode
    {
        Update,
        UpdateAndComplete,
        UpdateAndRemove
    }
}
