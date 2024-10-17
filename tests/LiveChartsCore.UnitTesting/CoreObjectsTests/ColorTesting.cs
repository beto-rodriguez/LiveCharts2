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
