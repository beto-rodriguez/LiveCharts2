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
using LiveChartsCore.Drawing.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LiveChartsCore.Measure;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LiveChartsCore.Kernel.Sketches;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines an Axis in a Cartesian chart.
    /// </summary>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <typeparam name="TTextGeometry">The type of the text geometry.</typeparam>
    /// <typeparam name="TLineGeometry">The type of the line geometry.</typeparam>
    /// <seealso cref="IAxis{TDrawingContext}" />
    /// <seealso cref="INotifyPropertyChanged" />
    public abstract class Axis<TDrawingContext, TTextGeometry, TLineGeometry> : UIElement<TDrawingContext>, IAxis<TDrawingContext>, INotifyPropertyChanged
        where TDrawingContext : DrawingContext
        where TTextGeometry : ILabelGeometry<TDrawingContext>, new()
        where TLineGeometry : ILineGeometry<TDrawingContext>, new()
    {
        #region fields

        /// <summary>
        /// The active separators
        /// </summary>
        protected readonly Dictionary<CartesianChart<TDrawingContext>, Dictionary<string, AxisVisualSeprator<TDrawingContext>>> activeSeparators = new();

        internal AxisOrientation _orientation;
        private double _minStep = 0;
        private Bounds? _dataBounds = null;
        private Bounds? _visibleDataBounds = null;
        private double _labelsRotation;
        // xo (x origin) and yo (y origin) are the distance to the center of the axis to the control bounds
        internal float _xo = 0f, _yo = 0f;
        private AxisPosition _position = AxisPosition.Start;
        private Func<double, string> _labeler = Labelers.Default;
        private Padding _padding = Padding.Default;
        private double? _minLimit = null;
        private double? _maxLimit = null;
        private IPaintTask<TDrawingContext>? _labelsPaint;
        private double _unitWidth = 1;
        private double _textSize = 16;
        private IPaintTask<TDrawingContext>? _separatorsPaint;
        private bool _showSeparatorLines = true;
        private bool _isVisible = true;
        private bool _isInverted;

        #endregion

        #region properties

        float IAxis.Xo { get => _xo; set => _xo = value; }
        float IAxis.Yo { get => _yo; set => _yo = value; }

        Bounds? IAxis.PreviousDataBounds { get; set; }

        Bounds? IAxis.PreviousVisibleDataBounds { get; set; }

        double? IAxis.PreviousMaxLimit { get; set; }

        double? IAxis.PreviousMinLimit { get; set; }

        Bounds IAxis.DataBounds => _dataBounds ?? throw new Exception("bounds not found");

        Bounds IAxis.VisibleDataBounds => _visibleDataBounds ?? throw new Exception("bounds not found");

        /// <inheritdoc cref="IAxis.Orientation"/>
        public AxisOrientation Orientation => _orientation;

        /// <inheritdoc cref="IAxis.Padding"/>
        public Padding Padding { get => _padding; set { _padding = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IAxis.Labeler"/>
        public Func<double, string> Labeler { get => _labeler; set { _labeler = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IAxis.MinStep"/>
        public double MinStep { get => _minStep; set { _minStep = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IAxis.MinLimit"/>
        public double? MinLimit { get => _minLimit; set { _minLimit = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IAxis.MaxLimit"/>
        public double? MaxLimit { get => _maxLimit; set { _maxLimit = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IAxis.UnitWidth"/>
        public double UnitWidth { get => _unitWidth; set { _unitWidth = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IAxis.Position"/>
        public AxisPosition Position { get => _position; set { _position = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IAxis.LabelsRotation"/>
        public double LabelsRotation { get => _labelsRotation; set { _labelsRotation = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IAxis.TextSize"/>
        public double TextSize { get => _textSize; set { _textSize = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IAxis.Labels"/>
        public IList<string>? Labels { get; set; }

        /// <inheritdoc cref="IAxis.ShowSeparatorLines"/>
        public bool ShowSeparatorLines { get => _showSeparatorLines; set { _showSeparatorLines = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IAxis.IsVisible"/>
        public bool IsVisible { get => _isVisible; set { _isVisible = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IAxis.IsInverted"/>
        public bool IsInverted { get => _isInverted; set { _isInverted = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IAxis{TDrawingContext}.LabelsPaint"/>
        public IPaintTask<TDrawingContext>? LabelsPaint
        {
            get => _labelsPaint;
            set => SetPaintProperty(ref _labelsPaint, value);
        }

        /// <inheritdoc cref="IAxis{TDrawingContext}.SeparatorsPaint"/>
        public IPaintTask<TDrawingContext>? SeparatorsPaint
        {
            get => _separatorsPaint;
            set => SetPaintProperty(ref _separatorsPaint, value);
        }

        /// <inheritdoc cref="IAxis{TDrawingContext}.TextBrush"/>
        [Obsolete("Renamed to LabelsPaint")]
        public IPaintTask<TDrawingContext>? TextBrush { get => LabelsPaint; set => LabelsPaint = value; }

        /// <inheritdoc cref="IAxis{TDrawingContext}.SeparatorsBrush"/>
        [Obsolete("Renamed to SeparatorsPaint")]
        public IPaintTask<TDrawingContext>? SeparatorsBrush { get => SeparatorsPaint; set => SeparatorsPaint = value; }

        /// <inheritdoc cref="IAxis.AnimationsSpeed"/>
        public TimeSpan? AnimationsSpeed { get; set; }

        /// <inheritdoc cref="IAxis.EasingFunction"/>
        public Func<float, float>? EasingFunction { get; set; }

        /// <inheritdoc cref="IAxis.IsNotifyingChanges"/>
        bool IAxis.IsNotifyingChanges { get; set; }

        #endregion

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <returns></returns>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <inheritdoc cref="IAxis{TDrawingContext}.Measure(CartesianChart{TDrawingContext})"/>
        public virtual void Measure(CartesianChart<TDrawingContext> chart)
        {
            if (_dataBounds == null) throw new Exception("DataBounds not found");

            var controlSize = chart.ControlSize;
            var drawLocation = chart.DrawMarginLocation;
            var drawMarginSize = chart.DrawMarginSize;

            var scale = new Scaler(drawLocation, drawMarginSize, this);
            var previousSacale = ((IAxis)this).PreviousDataBounds == null
                ? null
                : new Scaler(drawLocation, drawMarginSize, this, true);
            var axisTick = this.GetTick(drawMarginSize);

            var labeler = Labeler;
            if (Labels != null)
            {
                labeler = Labelers.BuildNamedLabeler(Labels).Function;
                _minStep = 1;
            }

            var s = axisTick.Value;
            if (s < _minStep) s = _minStep;

            if (LabelsPaint != null)
            {
                LabelsPaint.ZIndex = -1;
                chart.Canvas.AddDrawableTask(LabelsPaint);
            }
            if (SeparatorsPaint != null)
            {
                SeparatorsPaint.ZIndex = -1;
                SeparatorsPaint.SetClipRectangle(chart.Canvas, new RectangleF(drawLocation, drawMarginSize));
                chart.Canvas.AddDrawableTask(SeparatorsPaint);
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

            var max = MaxLimit == null ? (_visibleDataBounds ?? _dataBounds).Max : MaxLimit.Value;
            var min = MinLimit == null ? (_visibleDataBounds ?? _dataBounds).Min : MinLimit.Value;

            var start = Math.Truncate(min / s) * s;
            if (!activeSeparators.TryGetValue(chart, out var separators))
            {
                separators = new Dictionary<string, AxisVisualSeprator<TDrawingContext>>();
                activeSeparators[chart] = separators;
            }

            var measured = new HashSet<AxisVisualSeprator<TDrawingContext>>();

            for (var i = start; i <= max; i += s)
            {
                if (i < min) continue;

                var label = labeler(i);
                float x, y;
                if (_orientation == AxisOrientation.X)
                {
                    x = scale.ToPixels((float)i);
                    y = yoo;
                }
                else
                {
                    x = xoo;
                    y = scale.ToPixels((float)i);
                }

                if (!separators.TryGetValue(label, out var visualSeparator))
                {
                    visualSeparator = new AxisVisualSeprator<TDrawingContext>() { Value = (float)i };

                    if (LabelsPaint != null)
                    {
                        var textGeometry = new TTextGeometry { TextSize = size };
                        visualSeparator.Text = textGeometry;
                        if (hasRotation) textGeometry.Rotation = r;

                        _ = textGeometry
                            .TransitionateProperties(
                                nameof(textGeometry.X),
                                nameof(textGeometry.Y),
                                nameof(textGeometry.Opacity))
                            .WithAnimation(animation =>
                                animation
                                    .WithDuration(AnimationsSpeed ?? chart.AnimationsSpeed)
                                    .WithEasingFunction(EasingFunction ?? chart.EasingFunction));

                        if (previousSacale != null)
                        {
                            float xi, yi;

                            if (_orientation == AxisOrientation.X)
                            {
                                xi = previousSacale.ToPixels((float)i);
                                yi = yoo;
                            }
                            else
                            {
                                xi = xoo;
                                yi = previousSacale.ToPixels((float)i);
                            }

                            textGeometry.X = xi;
                            textGeometry.Y = yi;
                            textGeometry.CompleteAllTransitions();
                        }
                    }

                    if (SeparatorsPaint != null && ShowSeparatorLines)
                    {
                        var lineGeometry = new TLineGeometry();

                        visualSeparator.Line = lineGeometry;

                        _ = lineGeometry
                            .TransitionateProperties(
                                nameof(lineGeometry.X), nameof(lineGeometry.X1),
                                nameof(lineGeometry.Y), nameof(lineGeometry.Y1),
                                nameof(lineGeometry.Opacity))
                            .WithAnimation(animation =>
                                animation
                                    .WithDuration(AnimationsSpeed ?? chart.AnimationsSpeed)
                                    .WithEasingFunction(EasingFunction ?? chart.EasingFunction));

                        if (previousSacale != null)
                        {
                            float xi, yi;

                            if (_orientation == AxisOrientation.X)
                            {
                                xi = previousSacale.ToPixels((float)i);
                                yi = yoo;
                            }
                            else
                            {
                                xi = xoo;
                                yi = previousSacale.ToPixels((float)i);
                            }

                            if (_orientation == AxisOrientation.X)
                            {
                                lineGeometry.X = xi;
                                lineGeometry.X1 = xi;
                                lineGeometry.Y = lyi;
                                lineGeometry.Y1 = lyj;
                            }
                            else
                            {
                                lineGeometry.X = lxi;
                                lineGeometry.X1 = lxj;
                                lineGeometry.Y = yi;
                                lineGeometry.Y1 = yi;
                            }

                            lineGeometry.CompleteAllTransitions();
                        }
                    }

                    separators.Add(label, visualSeparator);
                }

                if (LabelsPaint != null && visualSeparator.Text != null)
                    LabelsPaint.AddGeometryToPaintTask(chart.Canvas, visualSeparator.Text);
                if (SeparatorsPaint != null && ShowSeparatorLines && visualSeparator.Line != null)
                    SeparatorsPaint.AddGeometryToPaintTask(chart.Canvas, visualSeparator.Line);

                if (visualSeparator.Text != null)
                {
                    visualSeparator.Text.Text = label;
                    visualSeparator.Text.Padding = _padding;
                    visualSeparator.Text.X = x;
                    visualSeparator.Text.Y = y;
                    if (hasRotation) visualSeparator.Text.Rotation = r;

                    if (((IAxis)this).PreviousDataBounds == null) visualSeparator.Text.CompleteAllTransitions();
                }

                if (visualSeparator.Line != null)
                {
                    if (_orientation == AxisOrientation.X)
                    {
                        visualSeparator.Line.X = x;
                        visualSeparator.Line.X1 = x;
                        visualSeparator.Line.Y = lyi;
                        visualSeparator.Line.Y1 = lyj;
                    }
                    else
                    {
                        visualSeparator.Line.X = lxi;
                        visualSeparator.Line.X1 = lxj;
                        visualSeparator.Line.Y = y;
                        visualSeparator.Line.Y1 = y;
                    }

                    if (((IAxis)this).PreviousDataBounds == null) visualSeparator.Line.CompleteAllTransitions();
                }

                if (visualSeparator.Text != null || visualSeparator.Line != null) _ = measured.Add(visualSeparator);
            }

            foreach (var separator in separators.ToArray())
            {
                if (measured.Contains(separator.Value)) continue;


                SoftDeleteSeparator(chart, separator.Value, scale);
                _ = separators.Remove(separator.Key);
            }
        }

        /// <inheritdoc cref="IAxis{TDrawingContext}.GetPossibleSize(CartesianChart{TDrawingContext})"/>
        public SizeF GetPossibleSize(CartesianChart<TDrawingContext> chart)
        {
            if (_dataBounds == null) throw new Exception("DataBounds not found");
            if (LabelsPaint == null) return new SizeF(0f, 0f);

            var ts = (float)TextSize;
            var labeler = Labeler;

            if (Labels != null)
            {
                labeler = Labelers.BuildNamedLabeler(Labels).Function;
                _minStep = 1;
            }

            var axisTick = this.GetTick(chart.DrawMarginSize);
            var s = axisTick.Value;
            if (s < _minStep) s = _minStep;

            var max = MaxLimit == null ? (_visibleDataBounds ?? _dataBounds).Max : MaxLimit.Value;
            var min = MinLimit == null ? (_visibleDataBounds ?? _dataBounds).Min : MinLimit.Value;

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
                    Rotation = r,
                    Padding = _padding
                };
                var m = textGeometry.Measure(LabelsPaint); // TextBrush.MeasureText(labeler(i, axisTick));
                if (m.Width > w) w = m.Width;
                if (m.Height > h) h = m.Height;
            }

            return new SizeF(w, h);
        }

        /// <inheritdoc cref="IAxis.Initialize(AxisOrientation)"/>
        public void Initialize(AxisOrientation orientation)
        {
            _orientation = orientation;
            _dataBounds = new Bounds();
            _visibleDataBounds = new Bounds();
        }

        /// <summary>
        /// Deletes the specified chart.
        /// </summary>
        /// <param name="chart">The chart.</param>
        /// <returns></returns>
        public virtual void Delete(CartesianChart<TDrawingContext> chart)
        {
            if (_labelsPaint != null)
            {
                chart.Canvas.RemovePaintTask(_labelsPaint);
                _labelsPaint.ClearGeometriesFromPaintTask(chart.Canvas);
            }
            if (_separatorsPaint != null)
            {
                chart.Canvas.RemovePaintTask(_separatorsPaint);
                _separatorsPaint.ClearGeometriesFromPaintTask(chart.Canvas);
            }

            _ = activeSeparators.Remove(chart);
        }

        /// <summary>
        /// Called when a property changes.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            if (!((IAxis)this).IsNotifyingChanges) return;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Softly deletes the separator.
        /// </summary>
        /// <param name="chart">The chart.</param>
        /// <param name="separator">The separator.</param>
        /// <param name="scale">The scale.</param>
        /// <returns></returns>
        protected virtual void SoftDeleteSeparator(
            Chart<TDrawingContext> chart,
            AxisVisualSeprator<TDrawingContext> separator,
            Scaler scale)
        {
            var controlSize = chart.ControlSize;
            var drawLocation = chart.DrawMarginLocation;
            var drawMarginSize = chart.DrawMarginSize;

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
                x = scale.ToPixels((float)separator.Value);
                y = yoo;
            }
            else
            {
                x = xoo;
                y = scale.ToPixels((float)separator.Value);
            }

            if (separator.Line != null)
            {
                if (_orientation == AxisOrientation.X)
                {
                    separator.Line.X = x;
                    separator.Line.X1 = x;
                    separator.Line.Y = lyi;
                    separator.Line.Y1 = lyj;
                }
                else
                {
                    separator.Line.X = lxi;
                    separator.Line.X1 = lxj;
                    separator.Line.Y = y;
                    separator.Line.Y1 = y;
                }

                separator.Line.Opacity = 0;
                separator.Line.RemoveOnCompleted = true;
            }

            if (separator.Text != null)
            {
                separator.Text.X = x;
                separator.Text.Y = y;
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
        protected override IPaintTask<TDrawingContext>?[] GetPaintTasks()
        {
            return new[] { _separatorsPaint, _labelsPaint };
        }
    }
}
