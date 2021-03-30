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
        private readonly HashSet<IChart> subscribedTo = new();
        private const float wedgeLength = 8;
        internal AxisOrientation orientation;
        private double? step = null;
        private Bounds? dataBounds = null;
        private Bounds? previousDataBounds = null;
        private double labelsRotation;
        private readonly Dictionary<string, AxisVisualSeprator<TDrawingContext>> activeSeparators = new();
        // xo (x origin) and yo (y origin) are the distance to the center of the axis to the control bounds
        internal float xo = 0f, yo = 0f;
        private AxisPosition position = AxisPosition.Start;
        private Func<double, AxisTick, string>? labeler;
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

        float IAxis.Xo { get => xo; set => xo = value; }
        float IAxis.Yo { get => yo; set => yo = value; }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <returns></returns>
        public event PropertyChangedEventHandler? PropertyChanged;

        List<IDrawableTask<TDrawingContext>> IAxis<TDrawingContext>.DeletingTasks => deletingTasks;

        /// <summary>
        /// Gets the previous data bounds.
        /// </summary>
        /// <value>
        /// The previous data bounds.
        /// </value>
        Bounds? IAxis.PreviousDataBounds => previousDataBounds;

        /// <summary>
        /// Gets the data bounds.
        /// </summary>
        /// <value>
        /// The data bounds.
        /// </value>
        Bounds IAxis.DataBounds => dataBounds ??= new Bounds();

        /// <summary>
        /// Gets the orientation.
        /// </summary>
        /// <value>
        /// The orientation.
        /// </value>
        public AxisOrientation Orientation { get => orientation; }

        /// <summary>
        /// Gets or sets the padding.
        /// </summary>
        /// <value>
        /// The padding.
        /// </value>
        public Padding Padding { get => padding; set { padding = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets the labeler.
        /// </summary>
        /// <value>
        /// The labeler.
        /// </value>
        public Func<double, AxisTick, string> Labeler { get => labeler ?? Labelers.Default; set { labeler = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets the step.
        /// </summary>
        /// <value>
        /// The step.
        /// </value>
        public double? Step { get => step; set { step = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        /// <value>
        /// The minimum value.
        /// </value>
        public double? MinLimit { get => minValue; set { minValue = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        /// <value>
        /// The maximum value.
        /// </value>
        public double? MaxLimit { get => maxValue; set { maxValue = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets the unit with. 
        /// </summary>
        /// <value>
        /// The unit with.
        /// </value>
        public double UnitWith { get => unitWith; set { unitWith = value; OnPropertyChanged(); } }
        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public AxisPosition Position { get => position; set { position = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets the labels rotation.
        /// </summary>
        /// <value>
        /// The labels rotation.
        /// </value>
        public double LabelsRotation { get => labelsRotation; set { labelsRotation = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets the size of the text.
        /// </summary>
        /// <value>
        /// The size of the text.
        /// </value>
        public double TextSize { get => textSize; set { textSize = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets the text brush.
        /// </summary>
        /// <value>
        /// The text brush.
        /// </value>
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

        /// <summary>
        /// Gets or sets the separators brush.
        /// </summary>
        /// <value>
        /// The separators brush.
        /// </value>
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

        /// <summary>
        /// Gets or sets a value indicating whether [show separator lines].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show separator lines]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowSeparatorLines { get => showSeparatorLines; set { showSeparatorLines = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets a value indicating whether [show separator wedges].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show separator wedges]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowSeparatorWedges { get => showSeparatorWedges; set { showSeparatorWedges = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is inverted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is inverted; otherwise, <c>false</c>.
        /// </value>
        public bool IsInverted { get => isInverted; set { isInverted = value; OnPropertyChanged(); } }

        /// <summary>
        /// Measures the axis for the specified chart.
        /// </summary>
        /// <param name="chart">The chart.</param>
        public void Measure(CartesianChart<TDrawingContext> chart)
        {
            if (dataBounds == null) throw new Exception("DataBounds not found");

            var controlSize = chart.ControlSize;
            var drawLocation = chart.DrawMaringLocation;
            var drawMarginSize = chart.DrawMarginSize;
            var labeler = Labeler;

            var scale = new Scaler(drawLocation, drawMarginSize, orientation, dataBounds, IsInverted);
            var previousSacale = previousDataBounds == null
                ? null
                : new Scaler(drawLocation, drawMarginSize, orientation, previousDataBounds, IsInverted);
            var axisTick = this.GetTick(drawMarginSize);

            var s = step == null || step == 0
                ? axisTick.Value
                : step.Value;

            if (TextBrush != null)
            {
                TextBrush.ZIndex = -1;
                chart.Canvas.AddDrawableTask(TextBrush);
            }
            if (SeparatorsBrush != null)
            {
                SeparatorsBrush.ZIndex = -1;
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

            for (var i = start; i <= dataBounds.max; i += s)
            {
                if (i < dataBounds.min) continue;

                var label = labeler(i, axisTick);
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

                if (!activeSeparators.TryGetValue(label, out var visualSeparator))
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

                    activeSeparators.Add(label, visualSeparator);
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

                if (visualSeparator.Text != null) chart.MeasuredDrawables.Add(visualSeparator.Text);
                if (visualSeparator.Line != null) chart.MeasuredDrawables.Add(visualSeparator.Line);
            }

            foreach (var separator in activeSeparators.ToArray())
            {
                var usedLabel = separator.Value.Text != null && chart.MeasuredDrawables.Contains(separator.Value.Text);
                var usedLine = separator.Value.Line != null && chart.MeasuredDrawables.Contains(separator.Value.Line);
                if (usedLine || usedLabel)
                {
                    continue;
                }

                float x, y;
                if (orientation == AxisOrientation.X)
                {
                    x = scale.ToPixels((float)separator.Value.Value);
                    y = yoo;
                }
                else
                {
                    x = xoo;
                    y = scale.ToPixels((float)separator.Value.Value);
                }

                if (separator.Value.Line != null)
                {
                    if (orientation == AxisOrientation.X)
                    {
                        separator.Value.Line.X = x;
                        separator.Value.Line.X1 = x;
                        separator.Value.Line.Y = lyi;
                        separator.Value.Line.Y1 = lyj;
                    }
                    else
                    {
                        separator.Value.Line.X = lxi;
                        separator.Value.Line.X1 = lxj;
                        separator.Value.Line.Y = y;
                        separator.Value.Line.Y1 = y;
                    }

                    separator.Value.Line.RemoveOnCompleted = true;
                }

                if (separator.Value.Text != null)
                {
                    separator.Value.Text.X = x;
                    separator.Value.Text.Y = y;
                    separator.Value.Text.RemoveOnCompleted = true;
                }

                activeSeparators.Remove(separator.Key);
            }
        }

        /// <summary>
        /// Gets the possible size for the given chart.
        /// </summary>
        /// <param name="chart">The chart.</param>
        /// <returns></returns>
        public SizeF GetPossibleSize(CartesianChart<TDrawingContext> chart)
        {
            if (dataBounds == null) throw new Exception("DataBounds not found");
            if (TextBrush == null) return new SizeF(0f, 0f);

            var ts = (float)TextSize;
            var labeler = Labeler;
            var axisTick = this.GetTick(chart.DrawMarginSize);
            var s = step == null || step == 0
                ? axisTick.Value
                : step.Value;
            var start = Math.Truncate(dataBounds.min / s) * s;

            var w = 0f;
            var h = 0f;
            var r = (float)LabelsRotation;

            for (var i = start; i <= dataBounds.max; i += s)
            {
                var textGeometry = new TTextGeometry
                {
                    Text = labeler(i, axisTick),
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

        /// <summary>
        /// Initializes the axis for the specified orientation.
        /// </summary>
        /// <param name="orientation">The orientation.</param>
        public void Initialize(AxisOrientation orientation)
        {
            this.orientation = orientation;
            previousDataBounds = dataBounds;
            dataBounds = new Bounds();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}