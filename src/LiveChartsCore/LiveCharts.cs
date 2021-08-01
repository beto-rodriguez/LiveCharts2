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

using LiveChartsCore.Kernel;
using System;

namespace LiveChartsCore
{
    /// <summary>
    /// LiveCharts global settings.
    /// </summary>
    public static class LiveCharts
    {
        /// <summary>
        /// Gets a value indicating whether LiveCharts should create a log as it renders the charts.
        /// </summary>
        public static bool EnableLogging { get; set; } = false;

        private static readonly object s_defaultPaintTask = new();

        /// <summary>
        /// Gets a value indicating whether this instance is configured.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is configured; otherwise, <c>false</c>.
        /// </value>
        public static bool IsConfigured { get; private set; } = false;

        /// <summary>
        /// Gets the current settings.
        /// </summary>
        /// <value>
        /// The current settings.
        /// </value>
        public static LiveChartsSettings CurrentSettings { get; } = new();

        /// <summary>
        /// Gets the bar series hover key.
        /// </summary>
        /// <value>
        /// The bar series hover key.
        /// </value>
        public static string BarSeriesHoverKey => nameof(BarSeriesHoverKey);

        /// <summary>
        /// Gets the heat bar series hover key.
        /// </summary>
        /// <value>
        /// The bar series hover key.
        /// </value>
        public static string HeatSeriesHoverState => nameof(HeatSeriesHoverState);

        /// <summary>
        /// Gets the stepline series hover key.
        /// </summary>
        /// <value>
        /// The stepline series hover key.
        /// </value>
        public static string StepLineSeriesHoverKey => nameof(StepLineSeriesHoverKey);

        /// <summary>
        /// Gets the line series hover key.
        /// </summary>
        /// <value>
        /// The line series hover key.
        /// </value>
        public static string LineSeriesHoverKey => nameof(LineSeriesHoverKey);

        /// <summary>
        /// Gets the pie series hover key.
        /// </summary>
        /// <value>
        /// The pie series hover key.
        /// </value>
        public static string PieSeriesHoverKey => nameof(PieSeriesHoverKey);

        /// <summary>
        /// Gets the scatter series hover key.
        /// </summary>
        /// <value>
        /// The scatter series hover key.
        /// </value>
        public static string ScatterSeriesHoverKey => nameof(ScatterSeriesHoverKey);

        /// <summary>
        /// Gets the stacked bar series hover key.
        /// </summary>
        /// <value>
        /// The stacked bar series hover key.
        /// </value>
        public static string StackedBarSeriesHoverKey => nameof(StackedBarSeriesHoverKey);

        /// <summary>
        /// The disable animations
        /// </summary>
        public static TimeSpan DisableAnimations = TimeSpan.FromMilliseconds(1);

        /// <summary>
        /// Configures LiveCharts.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">$"{nameof(LiveChartsSettings)} must not be null.</exception>
        public static void Configure(Action<LiveChartsSettings> configuration)
        {
            if (configuration is null) throw new NullReferenceException($"{nameof(LiveChartsSettings)} must not be null.");

            IsConfigured = true;
            configuration(CurrentSettings);
        }

        /// <summary>
        /// Defines a mapper for the given type.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="mapper">The mapper.</param>
        /// <returns></returns>
        public static LiveChartsSettings HasMapFor<TModel>(Action<TModel, ChartPoint> mapper)
        {
            return CurrentSettings.HasMap(mapper);
        }
    }
}
