using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.General.TemplatedTooltips;

public class ViewModel
{
    public ISeries[] Series { get; set; } = [
        new ColumnSeries<GeometryPoint>
        {
            Values = [
                new GeometryPoint { Value = 4, Geometry = SVGPoints.Star },
                new GeometryPoint { Value = 2, Geometry = SVGPoints.Square },
                new GeometryPoint { Value = 6, Geometry = SVGPoints.Circle },
                new GeometryPoint { Value = 3, Geometry = SVGPoints.Cross },
                new GeometryPoint { Value = 5, Geometry = SVGPoints.Diamond }
            ],
            // The Mapping property is used to map the data points to the series
            // to learn more about the Mapping property visit:
            // https://livecharts.dev/docs/{{ platform }}/{{ version }}/Overview.Mappers
            Mapping = (dataPoint, index) => new(index, dataPoint.Value)
        }
    ];
}

public class GeometryPoint
{
    public double Value { get; set; }
    public string? Geometry { get; set; }
}
