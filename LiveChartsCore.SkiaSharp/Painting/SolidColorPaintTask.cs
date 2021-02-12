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

using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharp.Drawing;
using LiveChartsCore.SkiaSharp.Transitions;
using LiveChartsCore.SkiaSharp.Transitions.Composed;
using LiveChartsCore.Transitions;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Painting
{
    public class SolidColorPaintTask : PaintTask
    {
        private readonly ColorTransitionProperty colorTransition;
        private readonly FloatTransitionProperty strokeMiterTransition;
        private readonly PathEffectTransition pathEffectTransition;
        private readonly ShaderTransition shaderTransition;

        public SolidColorPaintTask()
        {
            colorTransition = RegisterTransitionProperty(new ColorTransitionProperty(nameof(Color), new SKColor()));
            strokeMiterTransition = RegisterTransitionProperty(new FloatTransitionProperty(nameof(StrokeMiter), 0f));
            pathEffectTransition = RegisterTransitionProperty(new PathEffectTransition(nameof(PathEffect)));
            shaderTransition = RegisterTransitionProperty(new ShaderTransition(nameof(Shader)));
        }

        public SolidColorPaintTask(SKColor color)
        {
            colorTransition = RegisterTransitionProperty(
                new ColorTransitionProperty(nameof(Color), new SKColor(color.Red, color.Green, color.Blue, color.Alpha)));
            strokeMiterTransition = RegisterTransitionProperty(new FloatTransitionProperty(nameof(StrokeMiter), 0f));
            pathEffectTransition = RegisterTransitionProperty(new PathEffectTransition(nameof(PathEffect)));
            shaderTransition = RegisterTransitionProperty(new ShaderTransition(nameof(Shader)));
        }

        public SolidColorPaintTask(SKColor color, float strokeWidth)
        {
            colorTransition = RegisterTransitionProperty(
                new ColorTransitionProperty(nameof(Color), new SKColor(color.Red, color.Green, color.Blue, color.Alpha)));
            strokeWidthTransition = RegisterTransitionProperty(new FloatTransitionProperty(nameof(StrokeWidth), strokeWidth));
            strokeMiterTransition = RegisterTransitionProperty(new FloatTransitionProperty(nameof(StrokeMiter), 0f));
            pathEffectTransition = RegisterTransitionProperty(new PathEffectTransition(nameof(PathEffect)));
            shaderTransition = RegisterTransitionProperty(new ShaderTransition(nameof(Shader)));
        }

        public bool IsAntialias { get; set; } = true;
        public SKStrokeCap StrokeCap { get; set; }
        public SKStrokeJoin StrokeJoin { get; set; }
        public SKColor Color
        { 
            get => colorTransition.GetCurrentMovement(this); 
            set => colorTransition.MoveTo(value, this); 
        }
        public float StrokeMiter 
        {
            get => strokeMiterTransition.GetCurrentMovement(this); 
            set => strokeMiterTransition.MoveTo(value, this); 
        }
        public PathEffect PathEffect
        { 
            get => pathEffectTransition.GetCurrentMovement(this); 
            set => pathEffectTransition.MoveTo(value, this); 
        }
        public Shader Shader
        {
            get => shaderTransition.GetCurrentMovement(this);
            set => shaderTransition.MoveTo(value, this);
        }

        public override IDrawableTask<SkiaDrawingContext> CloneTask()
        {
            var clone = new SolidColorPaintTask
            {
                Style = Style,
                IsStroke = IsStroke,
                Color = Color,
                IsAntialias = IsAntialias,
                PathEffect = PathEffect,
                StrokeCap = StrokeCap,
                StrokeJoin = StrokeJoin,
                StrokeMiter = StrokeMiter,
                StrokeWidth = StrokeWidth
            };

            return clone;
        }

        public override void InitializeTask(SkiaDrawingContext drawingContext)
        {
            if (skiaPaint == null) skiaPaint = new SKPaint();

            skiaPaint.Color = Color;
            skiaPaint.IsAntialias = IsAntialias;
            skiaPaint.IsStroke = IsStroke;
            skiaPaint.StrokeCap = StrokeCap;
            skiaPaint.StrokeJoin = StrokeJoin;
            skiaPaint.StrokeMiter = StrokeMiter;
            skiaPaint.StrokeWidth = StrokeWidth;
            skiaPaint.Style = IsStroke ? SKPaintStyle.Stroke : SKPaintStyle.Fill;
            if (PathEffect != null) skiaPaint.PathEffect = PathEffect.GetSKPath();
            if (Shader != null) skiaPaint.Shader = Shader.GetSKShader();

            drawingContext.Paint = skiaPaint;
        }
    }
}
