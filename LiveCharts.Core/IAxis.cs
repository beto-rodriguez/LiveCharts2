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
using System;
using System.Collections.Generic;
using System.Drawing;

namespace LiveChartsCore
{
    public interface IAxis<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        Bounds DataBounds { get; }
        AxisOrientation Orientation { get; }
        float Xo { get; set; }
        float Yo { get; set; }

        Func<double, AxisTick, string> Labeler { get; set; }
        double Step { get ; set; }
        double UnitWith { get; set; }

        AxisPosition Position { get ; set; }
        double LabelsRotation { get; set; }

        IWritableTask<TDrawingContext> TextBrush { get; set; }

        IDrawableTask<TDrawingContext> SeparatorsBrush { get; set; }

        bool ShowSeparatorLines { get; set; }
        bool ShowSeparatorWedges { get; set; }

        IDrawableTask<TDrawingContext> AlternativeSeparatorForeground { get; set; }

        void Initialize(AxisOrientation orientation);
        void Measure(IChartView<TDrawingContext> view, HashSet<IDrawable<TDrawingContext>> drawBucket);
        SizeF GetPossibleSize(IChartView<TDrawingContext> view);
    }
}