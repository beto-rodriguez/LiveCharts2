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
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LiveChartsCore.SkiaSharpView.Painting
{
    /// <inheritdoc cref="IDrawableTask{TDrawingContext}" />
    public abstract class PaintTask : Animatable, IDisposable, IDrawableTask<SkiaSharpDrawingContext>
    {
        /// <summary>
        /// The skia paint
        /// </summary>
        protected SKPaint skiaPaint;

        /// <summary>
        /// The stroke width transition
        /// </summary>
        protected FloatMotionProperty strokeWidthTransition;
        private HashSet<IDrawable<SkiaSharpDrawingContext>> _geometries = new HashSet<IDrawable<SkiaSharpDrawingContext>>();
        private IDrawable<SkiaSharpDrawingContext>[] _actualGeometries = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaintTask"/> class.
        /// </summary>
        public PaintTask()
        {
            strokeWidthTransition = RegisterMotionProperty(new FloatMotionProperty(nameof(StrokeThickness), 0f));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaintTask"/> class.
        /// </summary>
        /// <param name="color">The color.</param>
        public PaintTask(SKColor color) : this()
        {
            Color = color;
        }

        double IDrawableTask<SkiaSharpDrawingContext>.ZIndex { get; set; }

        /// <inheritdoc cref="IDrawableTask{TDrawingContext}.StrokeThickness" />
        public float StrokeThickness { get => strokeWidthTransition.GetMovement(this); set => strokeWidthTransition.SetMovement(value, this); }

        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        /// <value>
        /// The style.
        /// </value>
        public SKPaintStyle Style { get; set; }

        /// <inheritdoc cref="IDrawableTask{TDrawingContext}.IsStroke" />
        public bool IsStroke { get; set; }

        /// <inheritdoc cref="IDrawableTask{TDrawingContext}.IsFill" />
        public bool IsFill { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is antialias.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is antialias; otherwise, <c>false</c>.
        /// </value>
        public bool IsAntialias { get; set; } = true;

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public SKColor Color { get; set; }

        /// <summary>
        /// Gets or sets the clip rectangle.
        /// </summary>
        /// <value>
        /// The clip rectangle.
        /// </value>
        public RectangleF ClipRectangle { get; set; } = RectangleF.Empty;

        /// <inheritdoc cref="IDrawableTask{TDrawingContext}.InitializeTask(TDrawingContext)" />
        public abstract void InitializeTask(SkiaSharpDrawingContext drawingContext);

        /// <inheritdoc cref="IDrawableTask{TDrawingContext}.GetGeometries" />
        public IEnumerable<IDrawable<SkiaSharpDrawingContext>> GetGeometries()
        {
            var g = _actualGeometries ?? (_actualGeometries = _geometries.ToArray());
            foreach (var item in g)
            {
                yield return item;
            }
        }

        /// <inheritdoc cref="IDrawableTask{TDrawingContext}.SetGeometries(HashSet{IDrawable{TDrawingContext}})" />
        public void SetGeometries(HashSet<IDrawable<SkiaSharpDrawingContext>> geometries)
        {
            _geometries = geometries;
            _actualGeometries = null;
            Invalidate();
        }

        /// <inheritdoc cref="IDrawableTask{TDrawingContext}.AddGeometyToPaintTask(IDrawable{TDrawingContext})" />
        public void AddGeometyToPaintTask(IDrawable<SkiaSharpDrawingContext> geometry)
        {
            _ = _geometries.Add(geometry);
            _actualGeometries = null;
            Invalidate();
        }

        /// <inheritdoc cref="IDrawableTask{TDrawingContext}.RemoveGeometryFromPainTask(IDrawable{TDrawingContext})" />
        public void RemoveGeometryFromPainTask(IDrawable<SkiaSharpDrawingContext> geometry)
        {
            _ = _geometries.Remove(geometry);
            _actualGeometries = null;
            Invalidate();
        }

        /// <inheritdoc cref="IDrawableTask{TDrawingContext}.CloneTask" />
        public abstract IDrawableTask<SkiaSharpDrawingContext> CloneTask();

        /// <inheritdoc cref="IDrawableTask{TDrawingContext}.SetOpacity(TDrawingContext, IGeometry{TDrawingContext})" />
        public abstract void SetOpacity(SkiaSharpDrawingContext context, IGeometry<SkiaSharpDrawingContext> geometry);

        /// <inheritdoc cref="IDrawableTask{TDrawingContext}.SetOpacity(TDrawingContext, IGeometry{TDrawingContext})" />
        public abstract void ResetOpacity(SkiaSharpDrawingContext context, IGeometry<SkiaSharpDrawingContext> geometry);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            skiaPaint?.Dispose();
            skiaPaint = null;
        }
    }
}
