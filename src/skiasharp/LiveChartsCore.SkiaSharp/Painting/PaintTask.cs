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
using LiveChartsCore.Drawing.Common;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using LiveChartsCore.SkiaSharpView.Painting.ImageFilters;
using LiveChartsCore.Threading;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LiveChartsCore.SkiaSharpView.Painting
{
    /// <inheritdoc cref="IPaintTask{TDrawingContext}" />
    public abstract class PaintTask : Animatable, IDisposable, IPaintTask<SkiaSharpDrawingContext>
    {
        private readonly FloatMotionProperty _strokeMiterTransition;
        private readonly Dictionary<object, HashSet<IDrawable<SkiaSharpDrawingContext>>> _geometriesByCanvas = new();
        private readonly Dictionary<object, RectangleF> _clipRectangles = new();

        /// <summary>
        /// The skia paint
        /// </summary>
        protected SKPaint? skiaPaint;

        /// <summary>
        /// The stroke width transition
        /// </summary>
        protected FloatMotionProperty strokeWidthTransition;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaintTask"/> class.
        /// </summary>
        protected PaintTask()
        {
            strokeWidthTransition = RegisterMotionProperty(new FloatMotionProperty(nameof(StrokeThickness), 0f));
            _strokeMiterTransition = RegisterMotionProperty(new FloatMotionProperty(nameof(StrokeMiter), 0f));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaintTask"/> class.
        /// </summary>
        /// <param name="color">The color.</param>
        protected PaintTask(SKColor color) : this()
        {
            Color = color;
        }

        double IPaintTask<SkiaSharpDrawingContext>.ZIndex { get; set; }

        /// <inheritdoc cref="IPaintTask{TDrawingContext}.StrokeThickness" />
        public float StrokeThickness { get => strokeWidthTransition.GetMovement(this); set => strokeWidthTransition.SetMovement(value, this); }

        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        /// <value>
        /// The style.
        /// </value>
        public SKPaintStyle Style { get; set; }

        /// <inheritdoc cref="IPaintTask{TDrawingContext}.IsStroke" />
        public bool IsStroke { get; set; }

        /// <inheritdoc cref="IPaintTask{TDrawingContext}.IsFill" />
        public bool IsFill { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is antialias.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is antialias; otherwise, <c>false</c>.
        /// </value>
        public bool IsAntialias { get; set; } = true;

        /// <summary>
        /// Gets or sets the stroke cap.
        /// </summary>
        /// <value>
        /// The stroke cap.
        /// </value>
        public SKStrokeCap StrokeCap { get; set; }

        /// <summary>
        /// Gets or sets the stroke join.
        /// </summary>
        /// <value>
        /// The stroke join.
        /// </value>
        public SKStrokeJoin StrokeJoin { get; set; }

        /// <summary>
        /// Gets or sets the stroke miter.
        /// </summary>
        /// <value>
        /// The stroke miter.
        /// </value>
        public float StrokeMiter
        {
            get => _strokeMiterTransition.GetMovement(this);
            set => _strokeMiterTransition.SetMovement(value, this);
        }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public SKColor Color { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is paused.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is paused; otherwise, <c>false</c>.
        /// </value>
        public bool IsPaused { get; set; }


        /// <summary>
        /// Gets or sets the path effect.
        /// </summary>
        /// <value>
        /// The path effect.
        /// </value>
        public PathEffect? PathEffect { get; set; }

        /// <summary>
        /// Gets or sets the image filer.
        /// </summary>
        /// <value>
        /// The image filer.
        /// </value>
        public ImageFilter? ImageFilter { get; set; }

        /// <inheritdoc cref="IPaintTask{TDrawingContext}.InitializeTask(TDrawingContext)" />
        public abstract void InitializeTask(SkiaSharpDrawingContext drawingContext);

        /// <inheritdoc cref="IPaintTask{TDrawingContext}.GetGeometries(MotionCanvas{TDrawingContext})" />
        public IEnumerable<IDrawable<SkiaSharpDrawingContext>> GetGeometries(MotionCanvas<SkiaSharpDrawingContext> canvas)
        {
            var enumerable = GetGeometriesByCanvas(canvas) ?? Enumerable.Empty<IDrawable<SkiaSharpDrawingContext>>();
            foreach (var item in enumerable)
            {
                yield return item;
            }
        }

        /// <inheritdoc cref="IPaintTask{TDrawingContext}.SetGeometries(MotionCanvas{TDrawingContext}, HashSet{IDrawable{TDrawingContext}})" />
        public void SetGeometries(MotionCanvas<SkiaSharpDrawingContext> canvas, HashSet<IDrawable<SkiaSharpDrawingContext>> geometries)
        {
            _geometriesByCanvas[canvas.Sync] = geometries;
            Invalidate();
        }

        /// <inheritdoc cref="IPaintTask{TDrawingContext}.AddGeometryToPaintTask(MotionCanvas{TDrawingContext}, IDrawable{TDrawingContext})" />
        public void AddGeometryToPaintTask(MotionCanvas<SkiaSharpDrawingContext> canvas, IDrawable<SkiaSharpDrawingContext> geometry)
        {
            var g = GetGeometriesByCanvas(canvas);
            if (g is null)
            {
                g = new HashSet<IDrawable<SkiaSharpDrawingContext>>();
                _geometriesByCanvas[canvas.Sync] = g;
            }
            _ = g.Add(geometry);
            Invalidate();
        }

        /// <inheritdoc cref="IPaintTask{TDrawingContext}.RemoveGeometryFromPainTask(MotionCanvas{TDrawingContext}, IDrawable{TDrawingContext})" />
        public void RemoveGeometryFromPainTask(MotionCanvas<SkiaSharpDrawingContext> canvas, IDrawable<SkiaSharpDrawingContext> geometry)
        {
            _ = GetGeometriesByCanvas(canvas)?.Remove(geometry);
            Invalidate();
        }

        /// <inheritdoc cref="IPaintTask{TDrawingContext}.ClearGeometriesFromPaintTask(MotionCanvas{TDrawingContext})"/>
        public void ClearGeometriesFromPaintTask(MotionCanvas<SkiaSharpDrawingContext> canvas)
        {
            GetGeometriesByCanvas(canvas)?.Clear();
            Invalidate();
        }

        /// <inheritdoc cref="IPaintTask{TDrawingContext}.GetClipRectangle(MotionCanvas{TDrawingContext})" />
        public RectangleF GetClipRectangle(MotionCanvas<SkiaSharpDrawingContext> canvas)
        {
            return _clipRectangles.TryGetValue(canvas.Sync, out var clip) ? clip : RectangleF.Empty;
        }

        /// <inheritdoc cref="IPaintTask{TDrawingContext}.SetClipRectangle(MotionCanvas{TDrawingContext}, RectangleF)" />
        public void SetClipRectangle(MotionCanvas<SkiaSharpDrawingContext> canvas, RectangleF value)
        {
            _clipRectangles[canvas.Sync] = value;
        }

        /// <inheritdoc cref="IPaintTask{TDrawingContext}.CloneTask" />
        public abstract IPaintTask<SkiaSharpDrawingContext> CloneTask();

        /// <inheritdoc cref="IPaintTask{TDrawingContext}.SetOpacity(TDrawingContext, IGeometry{TDrawingContext})" />
        public abstract void SetOpacity(SkiaSharpDrawingContext context, IGeometry<SkiaSharpDrawingContext> geometry);

        /// <inheritdoc cref="IPaintTask{TDrawingContext}.SetOpacity(TDrawingContext, IGeometry{TDrawingContext})" />
        public abstract void ResetOpacity(SkiaSharpDrawingContext context, IGeometry<SkiaSharpDrawingContext> geometry);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            skiaPaint?.Dispose();
            skiaPaint = null;
        }

        private HashSet<IDrawable<SkiaSharpDrawingContext>>? GetGeometriesByCanvas(MotionCanvas<SkiaSharpDrawingContext> canvas)
        {
            return _geometriesByCanvas.TryGetValue(canvas.Sync, out var geometries)
                ? geometries
                : null;
        }

    }
}
