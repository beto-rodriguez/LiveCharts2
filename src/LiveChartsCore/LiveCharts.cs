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
using System;

namespace LiveChartsCore
{
    public static class LiveCharts
    {
        private static bool isConfigured = false;
        private static object defaultPaintTask = new();
        private static readonly LiveChartsSettings settings = new();

        public static bool IsConfigured => isConfigured;
        public static LiveChartsSettings CurrentSettings => settings;

        public static string BarSeriesHoverKey => nameof(BarSeriesHoverKey);
        public static string LineSeriesHoverKey => nameof(LineSeriesHoverKey);
        public static string PieSeriesHoverKey => nameof(PieSeriesHoverKey);
        public static string ScatterSeriesHoverKey => nameof(ScatterSeriesHoverKey);
        public static string StackedBarSeriesHoverKey => nameof(StackedBarSeriesHoverKey);

        public static void Configure(Action<LiveChartsSettings> configuration)
        {
            if (configuration == null) throw new NullReferenceException($"{nameof(LiveChartsSettings)} must not be null.");

            isConfigured = true;
            configuration(settings);
        }

        public static LiveChartsSettings HasMapFor<TModel>(Action<IChartPoint, TModel, IChartPointContext> mapper)
        {
            return settings.HasMap(mapper);
        }
    }
}
