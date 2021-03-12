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

using LiveChartsCore.Context;
using LiveChartsCore.Drawing;
using LiveChartsCore.Drawing.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LiveChartsCore.Sketches
{
    public class Axis<TDrawingContext, TTextGeometry, TLineGeometry> : IAxis<TDrawingContext>
        where TDrawingContext : DrawingContext
        where TTextGeometry : ILabelGeometry<TDrawingContext>, new()
        where TLineGeometry : ILineGeometry<TDrawingContext>, new()
    {
        private const float wedgeLength = 8;
        internal AxisOrientation orientation;
        private double step = double.NaN;
        private Bounds dataBounds = new Bounds();
        private Bounds previousDataBounds = new Bounds();
        private double labelsRotation;
        private readonly Dictionary<string, AxisVisualSeprator<TDrawingContext>> activeSeparators =
            new Dictionary<string, AxisVisualSeprator<TDrawingContext>>();
        // xo (x origin) and yo (y origin) are the distance to the center of the axis to the control bounds
        internal float xo = 0f, yo = 0f;
        private AxisPosition position = AxisPosition.LeftOrBottom;
        private Func<double, AxisTick, string>? labeler;
        private Padding padding = new Padding { Left = 8, Top = 8, Bottom = 8, Right = 9 };

        public Bounds DataBounds
        {
            get => dataBounds;
            private set
            {
                previousDataBounds = dataBounds;
                dataBounds = value;
            }
        }

        public AxisOrientation Orientation { get => orientation; }
        float IAxis.Xo { get => xo; set => xo = value; }
        float IAxis.Yo { get => yo; set => yo = value; }

        public Padding Padding { get => padding; set => padding = value; }
        public Func<double, AxisTick, string> Labeler { get => labeler ?? Labelers.Default; set => labeler = value; }

        public double Step { get => step; set => step = value; }

        public double UnitWith { get; set; } = 1;

        public AxisPosition Position { get => position; set => position = value; }
        public double LabelsRotation { get => labelsRotation; set => labelsRotation = value; }

        public IDrawableTask<TDrawingContext>? TextBrush { get; set; }
        public double TextSize { get; set; } = 16;

        public IDrawableTask<TDrawingContext>? SeparatorsBrush { get; set; }

        public bool ShowSeparatorLines { get; set; } = true;
        public bool ShowSeparatorWedges { get; set; } = true;

        public IDrawableTask<TDrawingContext>? AlternativeSeparatorForeground { get; set; }

        public void Measure(CartesianChart<TDrawingContext> chart)
        {
            var controlSize = chart.ControlSize;
            var drawLocation = chart.DrawMaringLocation;
            var drawMarginSize = chart.DrawMarginSize;
            var labeler = Labeler;

            var scale = new ScaleContext(drawLocation, drawMarginSize, orientation, dataBounds);
            var axisTick = this.GetTick(drawMarginSize);

            var s = double.IsNaN(step) || step == 0
                ? axisTick.Value
                : step;

            if (TextBrush != null) chart.Canvas.AddDrawableTask(TextBrush);
            if (SeparatorsBrush != null) chart.Canvas.AddDrawableTask(SeparatorsBrush);

            var lyi = drawLocation.Y;
            var lyj = drawLocation.Y + drawMarginSize.Height;
            var lxi = drawLocation.X;
            var lxj = drawLocation.X + drawMarginSize.Width;

            float xoo = 0f, yoo = 0f;

            if (orientation == AxisOrientation.X)
            {
                yoo = position == AxisPosition.LeftOrBottom
                     ? controlSize.Height - yo
                     : yo;
            }
            else
            {
                xoo = position == AxisPosition.LeftOrBottom
                    ? xo
                    : controlSize.Width - xo;
            }

            var size = unchecked((float)TextSize);
            var r = unchecked((float)labelsRotation);
            var hasRotation = Math.Abs(r) > 0.01f;

            var start = Math.Truncate(dataBounds.min / s) * s;

            for (var i = start; i <= dataBounds.max; i += s)
            {
                if (i < dataBounds.min) continue;

                var label = labeler(i, axisTick);
                float x, y;
                if (orientation == AxisOrientation.X)
                {
                    x = scale.ScaleToUi(unchecked((float)i));
                    y = yoo;
                }
                else
                {
                    x = xoo;
                    y = scale.ScaleToUi(unchecked((float)i));
                }

                if (!activeSeparators.TryGetValue(label, out var visualSeparator))
                {
                    visualSeparator = new AxisVisualSeprator<TDrawingContext>();
                    if (TextBrush != null)
                    {
                        var textGeometry = new TTextGeometry();
                        textGeometry.TextSize = size;
                        visualSeparator.Text = textGeometry;
                        if (hasRotation) textGeometry.Rotation = r;

                        TextBrush.AddGeometyToPaintTask(textGeometry);
                    }
                    if (SeparatorsBrush != null)
                    {
                        var lineGeometry = new TLineGeometry();

                        if (orientation == AxisOrientation.X)
                        {
                            lineGeometry.X = x;
                            lineGeometry.X1 = x;
                            lineGeometry.Y = lyi;
                            lineGeometry.Y1 = lyj;
                        }
                        else
                        {
                            lineGeometry.X = lxi;
                            lineGeometry.X1 = lxj;
                            lineGeometry.Y = y;
                            lineGeometry.Y1 = y;
                        }

                        visualSeparator.Line = lineGeometry;
                        SeparatorsBrush.AddGeometyToPaintTask(lineGeometry);
                    }
                    activeSeparators.Add(label, visualSeparator);
                }

                if (visualSeparator.Text != null)
                {
                    visualSeparator.Text.Text = label;
                    visualSeparator.Text.Padding = padding;
                    visualSeparator.Text.X = x;
                    visualSeparator.Text.Y = y;
                    if (hasRotation) visualSeparator.Text.Rotation = r;
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
                }

                if (visualSeparator.Text != null) chart.MeasuredDrawables.Add(visualSeparator.Text);
                if (visualSeparator.Line != null) chart.MeasuredDrawables.Add(visualSeparator.Line);
            }

            foreach (var separator in activeSeparators.ToArray())
            {
                if (separator.Value.Line == null || chart.MeasuredDrawables.Contains(separator.Value.Line) ||
                    separator.Value.Text == null || chart.MeasuredDrawables.Contains(separator.Value.Text))
                {
                    continue;
                }

                activeSeparators.Remove(separator.Key);
            }
        }

        public SizeF GetPossibleSize(CartesianChart<TDrawingContext> chart)
        {
            if (TextBrush == null) return new SizeF(0f, 0f);

            var ts = (float)TextSize;
            var labeler = Labeler;
            var axisTick = this.GetTick(chart.DrawMarginSize);
            var s = double.IsNaN(step) || step == 0
                ? axisTick.Value
                : step;
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

        public void Initialize(AxisOrientation orientation)
        {
            this.orientation = orientation;
            DataBounds = new Bounds();
        }

        public IAxis<TDrawingContext> Copy()
        {
            return new Axis<TDrawingContext, TTextGeometry, TLineGeometry>
            {
                Labeler = labeler ?? Labelers.Default,
                Step = step,
                UnitWith = UnitWith,
                Position = position,
                LabelsRotation = labelsRotation,
                TextBrush = TextBrush,
                SeparatorsBrush = SeparatorsBrush,
                ShowSeparatorLines = ShowSeparatorLines,
                ShowSeparatorWedges = ShowSeparatorWedges,
                AlternativeSeparatorForeground = AlternativeSeparatorForeground
            };
        }
    }
}