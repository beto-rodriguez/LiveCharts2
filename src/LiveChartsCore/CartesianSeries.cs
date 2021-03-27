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
using System;
using LiveChartsCore.Measure;

namespace LiveChartsCore
{
    /// <inheritdoc cref="ICartesianSeries{TDrawingContext}"/>
    public abstract class CartesianSeries<TModel, TVisual, TLabel, TDrawingContext>
        : DrawableSeries<TModel, TVisual, TLabel, TDrawingContext>, IDisposable, ICartesianSeries<TDrawingContext>
        where TDrawingContext : DrawingContext
        where TVisual : class, IVisualChartPoint<TDrawingContext>, new()
        where TLabel : class, ILabelGeometry<TDrawingContext>, new()
    {
        private int scalesXAt;
        private int scalesYAt;

        public CartesianSeries(SeriesProperties properties) : base(properties) { }

        /// <inheritdoc cref="ICartesianSeries{TDrawingContext}.ScalesXAt"/>
        public int ScalesXAt { get => scalesXAt; set { scalesXAt = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="ICartesianSeries{TDrawingContext}.ScalesYAt"/>
        public int ScalesYAt { get => scalesYAt; set { scalesYAt = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="ICartesianSeries{TDrawingContext}.GetBounds(CartesianChart{TDrawingContext}, IAxis{TDrawingContext}, IAxis{TDrawingContext})"/>
        public virtual DimensinalBounds GetBounds(
            CartesianChart<TDrawingContext> chart, IAxis<TDrawingContext> x, IAxis<TDrawingContext> y)
        {
            if (dataProvider == null) throw new Exception("A data provider is required");

            return dataProvider.GetCartesianBounds(chart, this, x, y);
        }

        /// <inheritdoc cref="ICartesianSeries{TDrawingContext}.Measure(CartesianChart{TDrawingContext}, IAxis{TDrawingContext}, IAxis{TDrawingContext})"/>
        public abstract void Measure(
            CartesianChart<TDrawingContext> chart, IAxis<TDrawingContext> x, IAxis<TDrawingContext> y);

        public override void Delete(IChartView chart)
        {
            var core = (CartesianChart<TDrawingContext>)((IChartView<TDrawingContext>)chart).CoreChart;

            var secondaryAxis = core.XAxes[ScalesXAt];
            var primaryAxis = core.YAxes[ScalesYAt];

            var secondaryScale = new Scaler(
                core.DrawMaringLocation, core.DrawMarginSize, secondaryAxis.Orientation, secondaryAxis.DataBounds, secondaryAxis.IsInverted);
            var primaryScale = new Scaler(
                core.DrawMaringLocation, core.DrawMarginSize, primaryAxis.Orientation, primaryAxis.DataBounds, primaryAxis.IsInverted);

            foreach (var point in everFetched)
            {
                if (point.Context.Chart != chart) continue;

                SoftDeletePoint(point, primaryScale, secondaryScale);
            }
        }
    }
}
