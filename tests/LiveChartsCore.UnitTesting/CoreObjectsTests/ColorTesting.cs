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

using LiveChartsCore.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveChartsCore.UnitTesting.CoreObjectsTests;

[TestClass]
public class ColorTesting
{
    [TestMethod]
    public void Compare()
    {
        Assert.IsTrue(new LvcColor(123, 30, 29) != new LvcColor(123, 32, 23));
        Assert.IsTrue(new LvcColor(123, 30, 29) == new LvcColor(123, 30, 29));
        Assert.IsTrue(LvcColor.Empty != new LvcColor(123, 30, 29));
        Assert.IsTrue(LvcColor.Empty == LvcColor.Empty);

        var a = LvcColor.Empty;
        a.R = 1;
        a.G = 1;
        a.B = 1;
        a.A = 1;

        Assert.IsTrue(a != new LvcColor(1, 1, 1, 1));
    }
}
