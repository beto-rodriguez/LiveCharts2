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

using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveChartsCore.UnitTesting.CoreObjectsTests;

[TestClass]
public class LabelerTesting
{
    [TestMethod]
    public void Million()
    {
        string result;

        result = Labelers.SixRepresentativeDigits(100000000);
        Assert.IsTrue(result == "100 M");
        result = Labelers.SixRepresentativeDigits(10000000);
        Assert.IsTrue(result == "10 M");
        result = Labelers.SixRepresentativeDigits(1000000);
        Assert.IsTrue(result == "1 M");
        result = Labelers.SixRepresentativeDigits(100000);
        Assert.IsTrue(result == "100000");
        result = Labelers.SixRepresentativeDigits(-1000000);
        Assert.IsTrue(result == "-1 M");
        result = Labelers.SixRepresentativeDigits(-10000000);
        Assert.IsTrue(result == "-10 M");
        result = Labelers.SixRepresentativeDigits(-100000000);
        Assert.IsTrue(result == "-100 M");
    }

    [TestMethod]
    public void Micra()
    {
        string result;

        result = Labelers.SixRepresentativeDigits(0.00000001);
        Assert.IsTrue(result == "0.01 µ");
        result = Labelers.SixRepresentativeDigits(0.0000001);
        Assert.IsTrue(result == "0.1 µ");
        result = Labelers.SixRepresentativeDigits(0.000001);
        Assert.IsTrue(result == "1 µ");
        result = Labelers.SixRepresentativeDigits(0.00001);
        Assert.IsTrue(result == "0.00001");
        result = Labelers.SixRepresentativeDigits(-0.000001);
        Assert.IsTrue(result == "-1 µ");
        result = Labelers.SixRepresentativeDigits(-0.00000001);
        Assert.IsTrue(result == "-0.01 µ");
        result = Labelers.SixRepresentativeDigits(-0.0000001);
        Assert.IsTrue(result == "-0.1 µ");
    }

    [TestMethod]
    public void Currency()
    {
        string result;

        result = Labelers.FormatCurrency(-1000000000000000, ",", ".", NumberFormatInfo.CurrentInfo.CurrencySymbol);
        Assert.IsTrue(result == "-$1 Q");
        result = Labelers.FormatCurrency(-100000000000000, ",", ".", NumberFormatInfo.CurrentInfo.CurrencySymbol);
        Assert.IsTrue(result == "-$100 T");
        result = Labelers.FormatCurrency(-10000000000000, ",", ".", NumberFormatInfo.CurrentInfo.CurrencySymbol);
        Assert.IsTrue(result == "-$10 T");
        result = Labelers.FormatCurrency(-1000000000000, ",", ".", NumberFormatInfo.CurrentInfo.CurrencySymbol);
        Assert.IsTrue(result == "-$1 T");
        result = Labelers.FormatCurrency(-100000000000, ",", ".", NumberFormatInfo.CurrentInfo.CurrencySymbol);
        Assert.IsTrue(result == "-$100 B");
        result = Labelers.FormatCurrency(-10000000000, ",", ".", NumberFormatInfo.CurrentInfo.CurrencySymbol);
        Assert.IsTrue(result == "-$10 B");
        result = Labelers.FormatCurrency(-1000000000, ",", ".", NumberFormatInfo.CurrentInfo.CurrencySymbol);
        Assert.IsTrue(result == "-$1 B");
        result = Labelers.FormatCurrency(-100000000, ",", ".", NumberFormatInfo.CurrentInfo.CurrencySymbol);
        Assert.IsTrue(result == "-$100 M");
        result = Labelers.FormatCurrency(-10000000, ",", ".", NumberFormatInfo.CurrentInfo.CurrencySymbol);
        Assert.IsTrue(result == "-$10 M");
        result = Labelers.FormatCurrency(-1000000, ",", ".", NumberFormatInfo.CurrentInfo.CurrencySymbol);
        Assert.IsTrue(result == "-$1 M");
        result = Labelers.FormatCurrency(1000000, ",", ".", NumberFormatInfo.CurrentInfo.CurrencySymbol);
        Assert.IsTrue(result == "$1 M");
        result = Labelers.FormatCurrency(10000000, ",", ".", NumberFormatInfo.CurrentInfo.CurrencySymbol);
        Assert.IsTrue(result == "$10 M");
        result = Labelers.FormatCurrency(100000000, ",", ".", NumberFormatInfo.CurrentInfo.CurrencySymbol);
        Assert.IsTrue(result == "$100 M");
        result = Labelers.FormatCurrency(1000000000, ",", ".", NumberFormatInfo.CurrentInfo.CurrencySymbol);
        Assert.IsTrue(result == "$1 B");
        result = Labelers.FormatCurrency(10000000000, ",", ".", NumberFormatInfo.CurrentInfo.CurrencySymbol);
        Assert.IsTrue(result == "$10 B");
        result = Labelers.FormatCurrency(100000000000, ",", ".", NumberFormatInfo.CurrentInfo.CurrencySymbol);
        Assert.IsTrue(result == "$100 B");
        result = Labelers.FormatCurrency(1000000000000, ",", ".", NumberFormatInfo.CurrentInfo.CurrencySymbol);
        Assert.IsTrue(result == "$1 T");
        result = Labelers.FormatCurrency(10000000000000, ",", ".", NumberFormatInfo.CurrentInfo.CurrencySymbol);
        Assert.IsTrue(result == "$10 T");
        result = Labelers.FormatCurrency(100000000000000, ",", ".", NumberFormatInfo.CurrentInfo.CurrencySymbol);
        Assert.IsTrue(result == "$100 T");
        result = Labelers.FormatCurrency(1000000000000000, ",", ".", NumberFormatInfo.CurrentInfo.CurrencySymbol);
        Assert.IsTrue(result == "$1 Q");
    }
}
