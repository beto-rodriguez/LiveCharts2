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
using System;
using System.Drawing;
using LiveChartsCore.Measure;

namespace LiveChartsCore
{
    public interface IAxis
    {
        Bounds DataBounds { get; }
        AxisOrientation Orientation { get; }
        float Xo { get; set; }
        float Yo { get; set; }

        Func<double, AxisTick, string> Labeler { get; set; }
        double Step { get; set; }
        double UnitWith { get; set; }
        double? MinValue { get; set; }
        double? MaxValue { get; set; }
        bool IsInverted { get; set; }

        AxisPosition Position { get; set; }
        double LabelsRotation { get; set; }

        bool ShowSeparatorLines { get; set; }
        bool ShowSeparatorWedges { get; set; }

        void Initialize(AxisOrientation orientation);
    }

    public interface IAxis<TDrawingContext> : IAxis
        where TDrawingContext : DrawingContext
    {
        IDrawableTask<TDrawingContext>? TextBrush { get; set; }

        IDrawableTask<TDrawingContext>? SeparatorsBrush { get; set; }

        IDrawableTask<TDrawingContext>? AlternativeSeparatorForeground { get; set; }

        void Measure(CartesianChart<TDrawingContext> chart);
        SizeF GetPossibleSize(CartesianChart<TDrawingContext> chart);

        IAxis<TDrawingContext> Copy();
    }
}