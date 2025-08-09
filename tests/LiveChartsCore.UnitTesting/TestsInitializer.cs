using LiveChartsCore.SkiaSharpView;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveChartsCore.UnitTesting;

[TestClass]
public class TestsInitializer
{
    [AssemblyInitialize()]
    public static void MyTestInitialize(TestContext testContext)
    {
        // The test framework will call this method once -BEFORE- each test run.\

        LiveCharts.Configure(config => config.UseDefaults());
    }

    [AssemblyCleanup]
    public static void TearDown()
    {
        // The test framework will call this method once -AFTER- each test run.

    }
}
