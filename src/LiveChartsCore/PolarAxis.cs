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

using LiveChartsCore.Kernel;
using LiveChartsCore.Drawing;
using System.ComponentModel;
using LiveChartsCore.Kernel.Sketches;
using System.Collections.Generic;
using LiveChartsCore.Measure;
using System;
using LiveChartsCore.Drawing.Common;
using System.Runtime.CompilerServices;
using LiveChartsCore.Kernel.Helpers;
using System.Linq;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines an Axis in a Cartesian chart.
    /// </summary>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <typeparam name="TTextGeometry">The type of the text geometry.</typeparam>
    /// <typeparam name="TCircleGeometry">The type of the circle geometry.</typeparam>
    /// /// <typeparam name="TLineGeometry">The type of the line geometry.</typeparam>
    public abstract class PolarAxis<TDrawingContext, TTextGeometry, TLineGeometry, TCircleGeometry>
        : ChartElement<TDrawingContext>, IPolarAxis, IPlane<TDrawingContext>
            where TDrawingContext : DrawingContext
            where TTextGeometry : ILabelGeometry<TDrawingContext>, new()
            where TLineGeometry : ILineGeometry<TDrawingContext>, new()
            where TCircleGeometry : ISizedGeometry<TDrawingContext>, new()
    {
        #region fields

        /// <summary>
        /// The active separators
        /// </summary>
        protected readonly Dictionary<IChart, Dictionary<double, IVisualSeparator<TDrawingContext>>> activeSeparators = new();

        internal PolarAxisOrientation _orientation;
        private double _minStep = 0;
        private Bounds? _dataBounds = null;
        private Bounds? _visibleDataBounds = null;
        private double _labelsRotation;
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
        private bool _showSeparatorLines = true;
        private bool _isVisible = true;
        private bool _isInverted;
        private bool _forceStepToMin;

        #endregion

        #region properties

        Bounds? IPlane.PreviousDataBounds { get; set; }

        Bounds? IPlane.PreviousVisibleDataBounds { get; set; }

        double? IPlane.PreviousMaxLimit { get; set; }

        double? IPlane.PreviousMinLimit { get; set; }

        Bounds IPlane.DataBounds => _dataBounds ?? throw new Exception("bounds not found");

        Bounds IPlane.VisibleDataBounds => _visibleDataBounds ?? throw new Exception("bounds not found");

        /// <inheritdoc cref="IPlane.Name"/>
        public string? Name { get; set; } = null;

        /// <inheritdoc cref="IPlane.NameTextSize"/>
        public double NameTextSize { get => _nameTextSize; set { _nameTextSize = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IPlane.NamePadding"/>
        public Padding NamePadding { get => _namePadding; set { _namePadding = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IPolarAxis.Orientation"/>
        public PolarAxisOrientation Orientation => _orientation;

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

        /// <inheritdoc cref="IPlane.LabelsRotation"/>
        public double LabelsRotation { get => _labelsRotation; set { _labelsRotation = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IPlane.TextSize"/>
        public double TextSize { get => _textSize; set { _textSize = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IPlane.Labels"/>
        public IList<string>? Labels { get; set; }

        /// <inheritdoc cref="IPlane.ShowSeparatorLines"/>
        public bool ShowSeparatorLines { get => _showSeparatorLines; set { _showSeparatorLines = value; OnPropertyChanged(); } }

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
            set => SetPaintProperty(ref _separatorsPaint, value, true);
        }

        /// <inheritdoc cref="IPlane.AnimationsSpeed"/>
        public TimeSpan? AnimationsSpeed { get; set; }

        /// <inheritdoc cref="IPlane.EasingFunction"/>
        public Func<float, float>? EasingFunction { get; set; }

        /// <inheritdoc cref="IPlane.IsNotifyingChanges"/>
        bool IPlane.IsNotifyingChanges { get; set; }

        #endregion

        /// <inheritdoc cref="IPolarAxis.Initialized"/>
        public event Action<IPolarAxis>? Initialized;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <returns></returns>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <inheritdoc cref="ChartElement{TDrawingContext}.Measure(Chart{TDrawingContext})"/>
        public override void Measure(Chart<TDrawingContext> chart)
        {
            var polarChart = (PolarChart<TDrawingContext>)chart;

            if (_dataBounds is null) throw new Exception("DataBounds not found");

            var controlSize = polarChart.ControlSize;
            var drawLocation = polarChart.DrawMarginLocation;
            var drawMarginSize = polarChart.DrawMarginSize;

            var axisTick = this.GetTick(drawMarginSize);

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
                NamePaint.ZIndex = -1;
                polarChart.Canvas.AddDrawableTask(NamePaint);
            }
            if (LabelsPaint is not null)
            {
                LabelsPaint.ZIndex = -1;
                polarChart.Canvas.AddDrawableTask(LabelsPaint);
            }
            if (SeparatorsPaint is not null)
            {
                SeparatorsPaint.ZIndex = -1;
                SeparatorsPaint.SetClipRectangle(polarChart.Canvas, new LvcRectangle(drawLocation, drawMarginSize));
                polarChart.Canvas.AddDrawableTask(SeparatorsPaint);
            }

            IPolarAxis a, b;

            if (_orientation == PolarAxisOrientation.Angle)
            {
                a = this;
                b = polarChart.RadiusAxes[0];
            }
            else
            {
                a = polarChart.AngleAxes[0];
                b = this;
            }

            var scaler = new PolarScaler(polarChart.DrawMarginLocation, polarChart.DrawMarginSize, a, b, 0);

            var size = (float)TextSize;
            var r = (float)_labelsRotation;
            var hasRotation = Math.Abs(r) > 0.01f;

            var max = MaxLimit is null ? (_visibleDataBounds ?? _dataBounds).Max : MaxLimit.Value;
            var min = MinLimit is null ? (_visibleDataBounds ?? _dataBounds).Min : MinLimit.Value;

            var start = Math.Truncate(min / s) * s;

            if (!activeSeparators.TryGetValue(polarChart, out var separators))
            {
                separators = new Dictionary<double, IVisualSeparator<TDrawingContext>>();
                activeSeparators[polarChart] = separators;
            }

            //if (Name is not null && NamePaint is not null)
            //{
            //    if (_nameGeometry is null)
            //    {
            //        _nameGeometry = new TTextGeometry
            //        {
            //            TextSize = size,
            //            HorizontalAlign = Align.Middle,
            //            VerticalAlign = Align.Middle
            //        };
            //    }

            //    _nameGeometry.Padding = NamePadding;
            //    _nameGeometry.Text = Name;
            //    _nameGeometry.TextSize = (float)NameTextSize;

            //    if (_orientation == AxisOrientation.X)
            //    {
            //        var nameSize = _nameGeometry.Measure(NamePaint);
            //        _nameGeometry.X = (lxi + lxj) * 0.5f;
            //        _nameGeometry.Y = Position == AxisPosition.Start ? yoo + nameSize.Height : yoo - nameSize.Height;
            //    }
            //    else
            //    {
            //        _nameGeometry.RotateTransform = -90;
            //        var nameSize = _nameGeometry.Measure(NamePaint);
            //        _nameGeometry.X = Position == AxisPosition.Start ? xoo - nameSize.Width - Padding.Bottom : xoo + nameSize.Width + Padding.Bottom;
            //        _nameGeometry.Y = (lyi + lyj) * 0.5f;
            //    }
            //}

            var measured = new HashSet<IVisualSeparator<TDrawingContext>>();

            if (Orientation != PolarAxisOrientation.Angle)
            {
                var lnkmjvfds = 1;
            }

            for (var i = start; i <= max; i += s)
            {
                if (i < min) continue;

                var label = labeler(i);

                if (!separators.TryGetValue(i, out var visualSeparator))
                {
                    visualSeparator = _orientation == PolarAxisOrientation.Angle
                        ? new AxisVisualSeprator<TDrawingContext>() { Value = i }
                        : new RadialAxisVisualSeparator<TDrawingContext>() { Value = i };

                    if (LabelsPaint is not null)
                    {
                        var textGeometry = new TTextGeometry { TextSize = size };
                        visualSeparator.Text = textGeometry;
                        if (hasRotation) textGeometry.RotateTransform = r;

                        _ = textGeometry
                            .TransitionateProperties(
                                nameof(textGeometry.X),
                                nameof(textGeometry.Y),
                                nameof(textGeometry.Opacity))
                            .WithAnimation(animation =>
                                animation
                                    .WithDuration(AnimationsSpeed ?? polarChart.AnimationsSpeed)
                                    .WithEasingFunction(EasingFunction ?? polarChart.EasingFunction));

                        textGeometry.Opacity = 0;
                    }

                    if (SeparatorsPaint is not null && ShowSeparatorLines)
                    {
                        if (visualSeparator is AxisVisualSeprator<TDrawingContext> linearSeparator)
                        {
                            var lineGeometry = new TLineGeometry();

                            linearSeparator.Line = lineGeometry;

                            _ = lineGeometry
                                .TransitionateProperties(
                                    nameof(lineGeometry.X), nameof(lineGeometry.X1),
                                    nameof(lineGeometry.Y), nameof(lineGeometry.Y1),
                                    nameof(lineGeometry.Opacity))
                                .WithAnimation(animation =>
                                    animation
                                        .WithDuration(AnimationsSpeed ?? polarChart.AnimationsSpeed)
                                        .WithEasingFunction(EasingFunction ?? polarChart.EasingFunction));

                            lineGeometry.Opacity = 0;
                        }

                        if (visualSeparator is RadialAxisVisualSeparator<TDrawingContext> polarSeparator)
                        {
                            var circleGeometry = new TCircleGeometry();

                            polarSeparator.Circle = circleGeometry;

                            _ = circleGeometry
                                .TransitionateProperties(
                                    nameof(circleGeometry.X), nameof(circleGeometry.Y),
                                    nameof(circleGeometry.Width), nameof(circleGeometry.Height),
                                    nameof(circleGeometry.Opacity))
                                .WithAnimation(animation =>
                                    animation
                                        .WithDuration(AnimationsSpeed ?? polarChart.AnimationsSpeed)
                                        .WithEasingFunction(EasingFunction ?? polarChart.EasingFunction));

                            circleGeometry.Opacity = 0;
                        }
                    }

                    separators.Add(i, visualSeparator);
                }

                if (NamePaint is not null && _nameGeometry is not null)
                    NamePaint.AddGeometryToPaintTask(polarChart.Canvas, _nameGeometry);
                if (LabelsPaint is not null && visualSeparator.Text is not null)
                    LabelsPaint.AddGeometryToPaintTask(polarChart.Canvas, visualSeparator.Text);
                if (SeparatorsPaint is not null && ShowSeparatorLines && visualSeparator.Geometry is not null)
                    SeparatorsPaint.AddGeometryToPaintTask(polarChart.Canvas, visualSeparator.Geometry);

                var location = _orientation == PolarAxisOrientation.Angle
                        ? scaler.ToPixels(visualSeparator.Value, scaler.MaxRadius)
                        : scaler.ToPixels(0, visualSeparator.Value);

                if (visualSeparator.Text is not null)
                {
                    visualSeparator.Text.Text = label;
                    visualSeparator.Text.Padding = _padding;
                    visualSeparator.Text.X = location.X;
                    visualSeparator.Text.Y = location.Y;
                    if (hasRotation) visualSeparator.Text.RotateTransform = r;

                    visualSeparator.Text.Opacity = 1;

                    if (((IPolarAxis)this).PreviousDataBounds is null) visualSeparator.Text.CompleteAllTransitions();
                }

                if (visualSeparator.Geometry is not null)
                {
                    if (visualSeparator is AxisVisualSeprator<TDrawingContext> lineSepartator && lineSepartator.Line is not null)
                    {
                        lineSepartator.Line.X = scaler.CenterX;
                        lineSepartator.Line.X1 = location.X;
                        lineSepartator.Line.Y = scaler.CenterY;
                        lineSepartator.Line.Y1 = location.Y;

                        if (((IPolarAxis)this).PreviousDataBounds is null) lineSepartator.Line.CompleteAllTransitions();
                    }

                    if (visualSeparator is RadialAxisVisualSeparator<TDrawingContext> polarSeparator && polarSeparator.Circle is not null)
                    {
                        var radius = Math.Abs(location.X - scaler.CenterX);
                        polarSeparator.Circle.X = scaler.CenterX - radius;
                        polarSeparator.Circle.Y = scaler.CenterY - radius;
                        polarSeparator.Circle.Width = radius * 2;
                        polarSeparator.Circle.Height = radius * 2;

                        if (((IPolarAxis)this).PreviousDataBounds is null) polarSeparator.Circle.CompleteAllTransitions();
                    }

                    visualSeparator.Geometry.Opacity = 1;
                }

                if (visualSeparator.Text is not null || visualSeparator.Geometry is not null) _ = measured.Add(visualSeparator);
            }

            foreach (var separator in separators.ToArray())
            {
                if (measured.Contains(separator.Value)) continue;

                SoftDeleteSeparator(polarChart, separator.Value, scaler);
                _ = separators.Remove(separator.Key);
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
                //RotateTransform = Orientation == AxisOrientation.X ? 0 : -90,
                //Padding = Padding
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
            if (s < _minStep) s = _minStep;

            var max = MaxLimit is null ? (_visibleDataBounds ?? _dataBounds).Max : MaxLimit.Value;
            var min = MinLimit is null ? (_visibleDataBounds ?? _dataBounds).Min : MinLimit.Value;

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
                var m = textGeometry.Measure(LabelsPaint); // TextBrush.MeasureText(labeler(i, axisTick));
                if (m.Width > w) w = m.Width;
                if (m.Height > h) h = m.Height;
            }

            return new LvcSize(w, h);
        }

        /// <inheritdoc cref="IPolarAxis.Initialize(PolarAxisOrientation)"/>
        void IPolarAxis.Initialize(PolarAxisOrientation orientation)
        {
            _orientation = orientation;
            _dataBounds = new Bounds();
            _visibleDataBounds = new Bounds();
            Initialized?.Invoke(this);
        }

        /// <summary>
        /// Deletes the specified chart.
        /// </summary>
        /// <param name="chart">The chart.</param>
        /// <returns></returns>
        public virtual void Delete(Chart<TDrawingContext> chart)
        {

            if (_labelsPaint is not null)
            {
                chart.Canvas.RemovePaintTask(_labelsPaint);
                _labelsPaint.ClearGeometriesFromPaintTask(chart.Canvas);
            }
            if (_separatorsPaint is not null)
            {
                chart.Canvas.RemovePaintTask(_separatorsPaint);
                _separatorsPaint.ClearGeometriesFromPaintTask(chart.Canvas);
            }

            _ = activeSeparators.Remove(chart);
        }

        /// <inheritdoc cref="IChartElement{TDrawingContext}.RemoveFromUI(Chart{TDrawingContext})"/>
        public override void RemoveFromUI(Chart<TDrawingContext> chart)
        {
            base.RemoveFromUI(chart);
            _ = activeSeparators.Remove(chart);
        }

        /// <summary>
        /// Called when a property changes.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            if (!((IPolarAxis)this).IsNotifyingChanges) return;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Softly deletes the separator.
        /// </summary>
        /// <param name="chart">The chart.</param>
        /// <param name="separator">The separator.</param>
        /// <param name="scaler">The scale.</param>
        /// <returns></returns>
        protected virtual void SoftDeleteSeparator(
            Chart<TDrawingContext> chart,
            IVisualSeparator<TDrawingContext> separator,
            PolarScaler scaler)
        {
            if (separator.Geometry is null) return;

            var location = _orientation == PolarAxisOrientation.Angle
                ? scaler.ToPixels(separator.Value, scaler.MaxRadius)
                : scaler.ToPixels(0, separator.Value);

            if (separator is AxisVisualSeprator<TDrawingContext> lineSeparator)
            {
                lineSeparator.Line!.X = scaler.CenterX;
                lineSeparator.Line.Y = scaler.CenterY;
                lineSeparator.Line.X1 = location.X;
                lineSeparator.Line.Y1 = location.Y;
            }

            if (separator is RadialAxisVisualSeparator<TDrawingContext> polarSeparator)
            {
                polarSeparator.Circle!.X = scaler.CenterX;
                polarSeparator.Circle.Y = scaler.CenterY;
                polarSeparator.Circle.Width = 0;
                polarSeparator.Circle.Height = 0;
            }

            separator.Geometry.Opacity = 0;
            separator.Geometry.RemoveOnCompleted = true;

            if (separator.Text is not null)
            {
                separator.Text.X = 0;
                separator.Text.Y = 0;
                separator.Text.Opacity = 0;
                separator.Text.RemoveOnCompleted = true;
            }
        }

        /// <summary>
        /// Called when [paint changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected override void OnPaintChanged(string? propertyName)
        {
            OnPropertyChanged(propertyName);
        }

        /// <summary>
        /// Gets the paint tasks.
        /// </summary>
        /// <returns></returns>
        protected override IPaint<TDrawingContext>?[] GetPaintTasks()
        {
            return new[] { _separatorsPaint, _labelsPaint, _namePaint };
        }
    }
}
