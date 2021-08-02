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

using LiveChartsCore.Drawing;
using LiveChartsCore.Motion;
using LiveChartsCore.Threading;
using SkiaSharp;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LiveChartsCore.SkiaSharpView.Drawing.Geometries
{
    /// <summary>
    /// Defines a path geometry with a specified color.
    /// </summary>
    /// <seealso cref="PathGeometry" />
    public class PathShape : PathGeometry
    {
        private readonly ColorMotionProperty _strokeProperty;
        private readonly ColorMotionProperty _fillProperty;
        private readonly FloatMotionProperty _stProperty;

        /// <summary>
        /// Initializes a new instance of the <see cref="PathShape"/> class.
        /// </summary>
        public PathShape() : base()
        {
            _strokeProperty = RegisterMotionProperty(new ColorMotionProperty(nameof(StrokeColor), Color.FromArgb(0, 255, 255, 255)));
            _stProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(StrokeThickness)));
            _fillProperty = RegisterMotionProperty(new ColorMotionProperty(nameof(StrokeColor), Color.FromArgb(0, 255, 255, 255)));
        }

        /// <summary>
        /// Gets or sets the color of the stroke.
        /// </summary>
        /// <value>
        /// The color of the stroke.
        /// </value>
        public Color StrokeColor
        {
            get => _strokeProperty.GetMovement(this);
            set => _strokeProperty.SetMovement(value, this);
        }

        /// <summary>
        /// Gets or sets the stroke thickness.
        /// </summary>
        /// <value>
        /// The stroke thickness.
        /// </value>
        public float StrokeThickness
        {
            get => _stProperty.GetMovement(this);
            set => _stProperty.SetMovement(value, this);
        }

        /// <summary>
        /// Gets or sets the color of the fill.
        /// </summary>
        /// <value>
        /// The color of the fill.
        /// </value>
        public Color FillColor
        {
            get => _fillProperty.GetMovement(this);
            set => _fillProperty.SetMovement(value, this);
        }

        /// <inheritdoc cref="Draw(SkiaSharpDrawingContext)"/>
        public override void Draw(SkiaSharpDrawingContext context)
        {
            if (_commands.Count == 0) return;

            var toExecute = _drawingCommands ??= _commands.ToArray();

            var toRemoveSegments = new List<IPathCommand<SKPath>>();

            using var path = new SKPath();
            var isValid = true;

            foreach (var segment in toExecute)
            {
                segment.IsValid = true;
                segment.Execute(path, GetCurrentTime(), this);
                isValid = isValid && segment.IsValid;

                if (segment.IsValid && segment.RemoveOnCompleted) toRemoveSegments.Add(segment);
            }

            foreach (var segment in toRemoveSegments)
            {
                _ = _commands.Remove(segment);
                isValid = false;
            }

            if (IsClosed) path.Close();

            context.Paint.Color = FillColor.AsSKColor();
            context.Paint.StrokeWidth = 0;
            context.Paint.Style = SKPaintStyle.Fill;
            context.Canvas.DrawPath(path, context.Paint);

            context.Paint.Color = StrokeColor.AsSKColor();
            context.Paint.StrokeWidth = StrokeThickness;
            context.Paint.Style = SKPaintStyle.Stroke;
            context.Canvas.DrawPath(path, context.Paint);

            if (!isValid) Invalidate();
        }
    }
}
