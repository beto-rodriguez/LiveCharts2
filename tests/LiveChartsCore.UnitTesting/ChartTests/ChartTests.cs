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

using System.Net.NetworkInformation;
using LiveChartsCore.SkiaSharpView.SKCharts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveChartsCore.UnitTesting.ChartTests;

[TestClass]
public class ChartTests
{
    // based on https://github.com/beto-rodriguez/LiveCharts2/issues/1422

    // in theory LiveCharts properties should never be null, but we can't control the user input
    // specially on this case where DataContext could be null

    [TestMethod]
    public void CartesianShouldHandleNullParams()
    {
        // we are testing the properties defined on LiveChartsCore/Kernel/Sketches/ICartesianChartView.cs

        var chart = new SKCartesianChart
        {
            Width = 1000,
            Height = 1000,
            XAxes = null,
            YAxes = null,
            Sections = null,
            Series = null,
            DrawMarginFrame = null,
            VisualElements = null
        };

        var image = chart.GetImage();

        Assert.IsTrue(image is not null);
    }

    [TestMethod]
    public void PieShouldHandleNullParams()
    {
        // we are testing the properties defined on LiveChartsCore/Kernel/Sketches/IPieChartView.cs

        var chart = new SKPieChart
        {
            Width = 1000,
            Height = 1000,
            Series = null,
            VisualElements = null
        };

        var image = chart.GetImage();

        Assert.IsTrue(image is not null);
    }
}
