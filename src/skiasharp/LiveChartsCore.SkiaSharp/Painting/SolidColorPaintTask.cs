﻿// The MIT License(MIT)

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
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Motion;
using LiveChartsCore.SkiaSharpView.Motion.Composed;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Painting
{
    public class SolidColorPaintTask : PaintTask
    {
        private readonly ColorMotionProperty colorTransition;
        private readonly FloatMotionProperty strokeMiterTransition;
        private readonly PathEffectMotionProperty pathEffectTransition;
        private readonly ShaderMotionProperty shaderTransition;

        public SolidColorPaintTask()
        {
            colorTransition = RegisterMotionProperty(new ColorMotionProperty(nameof(Color), new SKColor()));
            strokeMiterTransition = RegisterMotionProperty(new FloatMotionProperty(nameof(StrokeMiter), 0f));
            pathEffectTransition = RegisterMotionProperty(new PathEffectMotionProperty(nameof(PathEffect)));
            shaderTransition = RegisterMotionProperty(new ShaderMotionProperty(nameof(Shader)));
        }

        public SolidColorPaintTask(SKColor color)
        {
            colorTransition = RegisterMotionProperty(
                new ColorMotionProperty(nameof(Color), new SKColor(color.Red, color.Green, color.Blue, color.Alpha)));
            strokeMiterTransition = RegisterMotionProperty(new FloatMotionProperty(nameof(StrokeMiter), 0f));
            pathEffectTransition = RegisterMotionProperty(new PathEffectMotionProperty(nameof(PathEffect)));
            shaderTransition = RegisterMotionProperty(new ShaderMotionProperty(nameof(Shader)));
        }

        public SolidColorPaintTask(SKColor color, float strokeWidth)
        {
            colorTransition = RegisterMotionProperty(
                new ColorMotionProperty(nameof(Color), new SKColor(color.Red, color.Green, color.Blue, color.Alpha)));
            strokeWidthTransition = RegisterMotionProperty(new FloatMotionProperty(nameof(StrokeWidth), strokeWidth));
            strokeMiterTransition = RegisterMotionProperty(new FloatMotionProperty(nameof(StrokeMiter), 0f));
            pathEffectTransition = RegisterMotionProperty(new PathEffectMotionProperty(nameof(PathEffect)));
            shaderTransition = RegisterMotionProperty(new ShaderMotionProperty(nameof(Shader)));
        }

        public bool IsAntialias { get; set; } = true;
        public SKStrokeCap StrokeCap { get; set; }
        public SKStrokeJoin StrokeJoin { get; set; }
        public SKColor Color
        { 
            get => colorTransition.GetMovement(this); 
            set => colorTransition.SetMovement(value, this); 
        }
        public float StrokeMiter 
        {
            get => strokeMiterTransition.GetMovement(this); 
            set => strokeMiterTransition.SetMovement(value, this); 
        }
        public PathEffect PathEffect
        { 
            get => pathEffectTransition.GetMovement(this); 
            set => pathEffectTransition.SetMovement(value, this); 
        }
        public Shader Shader
        {
            get => shaderTransition.GetMovement(this);
            set => shaderTransition.SetMovement(value, this);
        }

        public override IDrawableTask<SkiaSharpDrawingContext> CloneTask()
        {
            var clone = new SolidColorPaintTask
            {
                Style = Style,
                IsStroke = IsStroke,
                IsFill = IsFill,
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

        public override void InitializeTask(SkiaSharpDrawingContext drawingContext)
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
