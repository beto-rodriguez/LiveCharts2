using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace LiveChartsCore.UnitTesting.MockedObjects;

public class TestLabel : LabelGeometry
{
    public TestLabel()
    {
        Background = new Drawing.LvcColor(200, 200, 200, 50);
    }
}
