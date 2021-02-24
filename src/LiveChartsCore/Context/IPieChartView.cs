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

namespace LiveChartsCore.Context
{
    public interface IPieChartView<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        PieChart<TDrawingContext> Core { get; }
        Canvas<TDrawingContext> CoreCanvas { get; }

        System.Drawing.SizeF ControlSize { get; }

        IPieSeries<TDrawingContext> Series { get; set; }

        LegendPosition LegendPosition { get; set; }
        LegendOrientation LegendOrientation { get; set; }
        IChartLegend<TDrawingContext> Legend { get; }

        TooltipPosition TooltipPosition { get; set; }
        TooltipFindingStrategy TooltipFindingStrategy { get; set; }
        IChartTooltip<TDrawingContext> Tooltip { get; }

        Margin DrawMargin { get; set; }

        TimeSpan AnimationsSpeed { get; set; }

        Func<float, float> EasingFunction { get; set; }
    }
}
