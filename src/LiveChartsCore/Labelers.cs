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
using System.Collections.Generic;
using System.Globalization;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines common functions to build labels in a chart.
    /// </summary>
    public static class Labelers
    {
        static Labelers()
        {
            Default = Log10_7;
        }

        /// <summary>
        /// Gets the default labeler.
        /// </summary>
        /// <value>
        /// The default.
        /// </value>
        public static Func<double, string> Default { get; private set; }

        /// <summary>
        /// Gets the seven representative digits labeler.
        /// </summary>
        /// <value>
        /// The seven representative digits.
        /// </value>
        public static Func<double, string> SevenRepresentativeDigits => Log10_7;

        /// <summary>
        /// Gets the currency labeler.
        /// </summary>
        /// <value>
        /// The currency.
        /// </value>
        public static Func<double, string> Currency =>
            value => FormatCurrency(value, ",", ".", NumberFormatInfo.CurrentInfo.CurrencySymbol);

        /// <summary>
        /// Sets the default labeler.
        /// </summary>
        /// <param name="labeler">The labeler.</param>
        public static void SetDefaultLabeler(Func<double, string> labeler)
        {
            Default = labeler;
        }

        /// <summary>
        /// Formats to currency.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="thousands">The thousands.</param>
        /// <param name="decimals">The decimals.</param>
        /// <param name="symbol">The symbol.</param>
        /// <returns></returns>
        public static string FormatCurrency(double value, string thousands, string decimals, string symbol)
        {
            var l = value == 0 ? 0 : (int)Math.Log10(Math.Abs(value));
            var u = "";

            if (l > 12)
            {
                value /= Math.Pow(10, 12);
                u = "T";
            }
            if (l > 10)
            {
                value /= Math.Pow(10, 9);
                u = "G";
            }
            else if (l > 7)
            {
                value /= Math.Pow(10, 6);
                u = "M";
            }

            return value.ToString($"{symbol} #{thousands}###{thousands}##0{decimals}## {u}");
        }

        /// <summary>
        /// Builds a named labeler.
        /// </summary>
        /// <param name="labels">The labels.</param>
        /// <returns></returns>
        public static NamedLabeler BuildNamedLabeler(IList<string> labels)
        {
            return new(labels);
        }

        private static string Log10_7(double value)
        {
            var l = value == 0 ? 0 : (int)Math.Log10(Math.Abs(value));
            var u = "";

            if (l > 7)
            {
                if (value > 0)
                {
                    value /= Math.Pow(10, 6);
                    u = "M";
                }
                else
                {
                    value *= Math.Pow(10, 6);
                    u = "µ";
                }
            }

            return value.ToString($"######0.####### {u}");
        }

    }
}
