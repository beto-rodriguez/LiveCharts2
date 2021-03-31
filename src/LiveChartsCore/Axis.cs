// The MIT License(MIT)

// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

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

namespace LiveChartsCore
{
    public class Axis<TDrawingContext, TTextGeometry, TLineGeometry> : IAxis<TDrawingContext>, INotifyPropertyChanged
        where TDrawingContext : DrawingContext
        where TTextGeometry : ILabelGeometry<TDrawingContext>, new()
        where TLineGeometry : ILineGeometry<TDrawingContext>, new()
    {
        #region fields

        protected readonly HashSet<IChart> subscribedTo = new();
        private const float wedgeLength = 8;
        internal AxisOrientation orientation;
        private double minStep = 0;
        private Bounds? dataBounds = null;
        private Bounds? previousDataBounds = null;
        private double labelsRotation;
        private readonly Dictionary<CartesianChart<TDrawingContext>, Dictionary<string, AxisVisualSeprator<TDrawingContext>>> activeSeparators = new();
        // xo (x origin) and yo (y origin) are the distance to the center of the axis to the control bounds
        internal float xo = 0f, yo = 0f;
        private AxisPosition position = AxisPosition.Start;
        private Func<double, string> labeler = Labelers.Default;
        private Padding padding = new() { Left = 8, Top = 8, Bottom = 8, Right = 9 };
        private double? minValue = null;
        private double? maxValue = null;
        private IDrawableTask<TDrawingContext>? textBrush;
        private double unitWith = 1;
        protected List<IDrawableTask<TDrawingContext>> deletingTasks = new();
        private double textSize = 16;
        private IDrawableTask<TDrawingContext>? separatorsBrush;
        private bool showSeparatorLines = true;
        private bool showSeparatorWedges = true;
        private bool isInverted;

        #endregion

        #region properties

        float IAxis.Xo { get => xo; set => xo = value; }
        float IAxis.Yo { get => yo; set => yo = value; }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <returns></returns>
        public event PropertyChangedEventHandler? PropertyChanged;

        List<IDrawableTask<TDrawingContext>> IAxis<TDrawingContext>.DeletingTasks => deletingTasks;

        /// <inheritdoc cref="IAxis.PreviousDataBounds"/>
        Bounds? IAxis.PreviousDataBounds => previousDataBounds;

        /// <inheritdoc cref="IAxis.DataBounds"/>
        Bounds IAxis.DataBounds => dataBounds ??= new Bounds();

        /// <inheritdoc cref="IAxis.Orientation"/>
        public AxisOrientation Orientation { get => orientation; }

        /// <inheritdoc cref="IAxis.Padding"/>
        public Padding Padding { get => padding; set { padding = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IAxis.Labeler"/>
        public Func<double, string> Labeler { get => labeler; set { labeler = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IAxis.MinStep"/>
        public double MinStep { get => minStep; set { minStep = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IAxis.MinLimit"/>
        public double? MinLimit { get => minValue; set { minValue = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IAxis.MaxLimit"/>
        public double? MaxLimit { get => maxValue; set { maxValue = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IAxis.UnitWith"/>
        public double UnitWith { get => unitWith; set { unitWith = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IAxis.Position"/>
        public AxisPosition Position { get => position; set { position = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IAxis.LabelsRotation"/>
        public double LabelsRotation { get => labelsRotation; set { labelsRotation = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IAxis.TextSize"/>
        public double TextSize { get => textSize; set { textSize = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IAxis.Labels"/>
        public IList<string>? Labels { get; set; }

        /// <inheritdoc cref="IAxis.ShowSeparatorLines"/>
        public bool ShowSeparatorLines { get => showSeparatorLines; set { showSeparatorLines = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IAxis.ShowSeparatorWedges"/>
        public bool ShowSeparatorWedges { get => showSeparatorWedges; set { showSeparatorWedges = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IAxis.IsInverted"/>
        public bool IsInverted { get => isInverted; set { isInverted = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IAxis{TDrawingContext}.TextBrush"/>
        public IDrawableTask<TDrawingContext>? TextBrush
        {
            get => textBrush;
            set
            {
                if (textBrush != null) deletingTasks.Add(textBrush);
                textBrush = value;
                OnPropertyChanged();
            }
        }

        /// <inheritdoc cref="IAxis{TDrawingContext}.SeparatorsBrush"/>
        public IDrawableTask<TDrawingContext>? SeparatorsBrush
        {
            get => separatorsBrush;
            set
            {
                if (separatorsBrush != null) deletingTasks.Add(separatorsBrush);
                separatorsBrush = value;
                OnPropertyChanged();
            }
        }

        #endregion

        /// <inheritdoc cref="IAxis{TDrawingContext}.Measure(CartesianChart{TDrawingContext})"/>
        public void Measure(CartesianChart<TDrawingContext> chart)
        {
            if (dataBounds == null) throw new Exception("DataBounds not found");

            subscribedTo.Add(chart);

            var controlSize = chart.ControlSize;
            var drawLocation = chart.DrawMaringLocation;
            var drawMarginSize = chart.DrawMarginSize;

            var scale = new Scaler(drawLocation, drawMarginSize, orientation, dataBounds, IsInverted);
            var previousSacale = previousDataBounds == null
                ? null
                : new Scaler(drawLocation, drawMarginSize, orientation, previousDataBounds, IsInverted);
            var axisTick = this.GetTick(drawMarginSize);

            var labeler = Labeler;
            if (Labels != null)
            {
                labeler = Labelers.BuildNamedLabeler(Labels).Function;
                minStep = 1;
            }

            var s = axisTick.Value;
            if (s < minStep) s = minStep;

            if (TextBrush != null)
            {
                TextBrush.ZIndex = -1;
                chart.Canvas.AddDrawableTask(TextBrush);
            }
            if (SeparatorsBrush != null)
            {
                SeparatorsBrush.ZIndex = -1;
                SeparatorsBrush.ClipRectangle = new RectangleF(drawLocation, drawMarginSize);
                chart.Canvas.AddDrawableTask(SeparatorsBrush);
            }

            var lyi = drawLocation.Y;
            var lyj = drawLocation.Y + drawMarginSize.Height;
            var lxi = drawLocation.X;
            var lxj = drawLocation.X + drawMarginSize.Width;

            float xoo = 0f, yoo = 0f;

            if (orientation == AxisOrientation.X)
            {
                yoo = position == AxisPosition.Start
                     ? controlSize.Height - yo
                     : yo;
            }
            else
            {
                xoo = position == AxisPosition.Start
                    ? xo
                    : controlSize.Width - xo;
            }

            var size = (float)TextSize;
            var r = (float)labelsRotation;
            var hasRotation = Math.Abs(r) > 0.01f;

            var start = Math.Truncate(dataBounds.min / s) * s;
            if (!activeSeparators.TryGetValue(chart, out var separators))
            {
                separators = new Dictionary<string, AxisVisualSeprator<TDrawingContext>>();
                activeSeparators[chart] = separators;
            }

            var measured = new HashSet<AxisVisualSeprator<TDrawingContext>>();

            for (var i = start; i <= dataBounds.max; i += s)
            {
                if (i < dataBounds.min) continue;

                var label = labeler(i);
                float x, y;
                if (orientation == AxisOrientation.X)
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

                    if (TextBrush != null)
                    {
                        var textGeometry = new TTextGeometry { TextSize = size };
                        visualSeparator.Text = textGeometry;
                        if (hasRotation) textGeometry.Rotation = r;

                        textGeometry
                            .TransitionateProperties(
                                nameof(textGeometry.X),
                                nameof(textGeometry.Y))
                            .WithAnimation(animation =>
                                animation
                                    .WithDuration(chart.AnimationsSpeed)
                                    .WithEasingFunction(chart.EasingFunction));

                        if (previousSacale != null)
                        {
                            float xi, yi;

                            if (orientation == AxisOrientation.X)
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

                    if (SeparatorsBrush != null)
                    {
                        var lineGeometry = new TLineGeometry();

                        visualSeparator.Line = lineGeometry;

                        lineGeometry
                            .TransitionateProperties(
                                nameof(lineGeometry.X), nameof(lineGeometry.X1),
                                nameof(lineGeometry.Y), nameof(lineGeometry.Y1))
                            .WithAnimation(animation =>
                                animation
                                    .WithDuration(chart.AnimationsSpeed)
                                    .WithEasingFunction(chart.EasingFunction));

                        if (previousSacale != null)
                        {
                            float xi, yi;

                            if (orientation == AxisOrientation.X)
                            {
                                xi = previousSacale.ToPixels((float)i);
                                yi = yoo;
                            }
                            else
                            {
                                xi = xoo;
                                yi = previousSacale.ToPixels((float)i);
                            }

                            if (orientation == AxisOrientation.X)
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

                if (TextBrush != null && visualSeparator.Text != null) TextBrush.AddGeometyToPaintTask(visualSeparator.Text);
                if (SeparatorsBrush != null && visualSeparator.Line != null) SeparatorsBrush.AddGeometyToPaintTask(visualSeparator.Line);

                if (visualSeparator.Text != null)
                {
                    visualSeparator.Text.Text = label;
                    visualSeparator.Text.Padding = padding;
                    visualSeparator.Text.X = x;
                    visualSeparator.Text.Y = y;
                    if (hasRotation) visualSeparator.Text.Rotation = r;

                    if (previousDataBounds == null) visualSeparator.Text.CompleteAllTransitions();

                }

                if (visualSeparator.Line != null)
                {
                    if (orientation == AxisOrientation.X)
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

                    if (previousDataBounds == null) visualSeparator.Line.CompleteAllTransitions();
                }

                if (visualSeparator.Text != null || visualSeparator.Line != null) measured.Add(visualSeparator);
            }

            foreach (var separator in separators.ToArray())
            {
                if (measured.Contains(separator.Value)) continue;


                SoftDeleteSeparator(chart, separator.Value, scale);
                separators.Remove(separator.Key);
            }
        }

        /// <inheritdoc cref="IAxis{TDrawingContext}.GetPossibleSize(CartesianChart{TDrawingContext})"/>
        public SizeF GetPossibleSize(CartesianChart<TDrawingContext> chart)
        {
            if (dataBounds == null) throw new Exception("DataBounds not found");
            if (TextBrush == null) return new SizeF(0f, 0f);

            var ts = (float)TextSize;
            var labeler = Labeler;

            if (Labels != null)
            {
                labeler = Labelers.BuildNamedLabeler(Labels).Function;
                minStep = 1;
            }

            var axisTick = this.GetTick(chart.DrawMarginSize);
            var s = axisTick.Value;
            if (s < minStep) s = minStep;

            var start = Math.Truncate(dataBounds.min / s) * s;

            var w = 0f;
            var h = 0f;
            var r = (float)LabelsRotation;

            for (var i = start; i <= dataBounds.max; i += s)
            {
                var textGeometry = new TTextGeometry
                {
                    Text = labeler(i),
                    TextSize = ts,
                    Rotation = r,
                    Padding = padding
                };
                var m = textGeometry.Measure(TextBrush); // TextBrush.MeasureText(labeler(i, axisTick));
                if (m.Width > w) w = m.Width;
                if (m.Height > h) h = m.Height;
            }

            return new SizeF(w, h);
        }

        /// <inheritdoc cref="IAxis.Initialize(AxisOrientation)"/>
        public void Initialize(AxisOrientation orientation)
        {
            this.orientation = orientation;
            previousDataBounds = dataBounds;
            dataBounds = new Bounds();
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public virtual void Dispose()
        {
            foreach (var chart in subscribedTo.ToArray())
            {
                var cartesianChart = (CartesianChart<TDrawingContext>)chart;
                var canvas = cartesianChart.View.CoreCanvas;
                if (textBrush != null) canvas.RemovePaintTask(textBrush);
                if (separatorsBrush != null) canvas.RemovePaintTask(separatorsBrush);
                activeSeparators.Remove(cartesianChart);
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SoftDeleteSeparator(
            Chart<TDrawingContext> chart,
            AxisVisualSeprator<TDrawingContext> separator,
            Scaler scale)
        {
            var controlSize = chart.ControlSize;
            var drawLocation = chart.DrawMaringLocation;
            var drawMarginSize = chart.DrawMarginSize;

            var lyi = drawLocation.Y;
            var lyj = drawLocation.Y + drawMarginSize.Height;
            var lxi = drawLocation.X;
            var lxj = drawLocation.X + drawMarginSize.Width;

            float xoo = 0f, yoo = 0f;

            if (orientation == AxisOrientation.X)
            {
                yoo = position == AxisPosition.Start
                     ? controlSize.Height - yo
                     : yo;
            }
            else
            {
                xoo = position == AxisPosition.Start
                    ? xo
                    : controlSize.Width - xo;
            }

            float x, y;
            if (orientation == AxisOrientation.X)
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
                if (orientation == AxisOrientation.X)
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

                separator.Line.RemoveOnCompleted = true;
            }

            if (separator.Text != null)
            {
                separator.Text.X = x;
                separator.Text.Y = y;
                separator.Text.RemoveOnCompleted = true;
            }
        }
    }
}