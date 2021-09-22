using System.Collections.Generic;
using LiveChartsCore.Geo;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace ViewModelsSamples.Maps.World
{
    public class ViewModel
    {
        // every country has a unique identifier
        // check the "shortName" property in the following
        // json file to assign a value to a country in the heat map
        // https://github.com/beto-rodriguez/LiveCharts2/blob/master/docs/_assets/word-map-index.json

        public IEnumerable<MapShape<SkiaSharpDrawingContext>> Shapes { get; set; }
            = new HeatLand[]
            {
                new HeatLand { Name = "mex", Value = 10 },
                new HeatLand { Name = "usa", Value = 15 },
                new HeatLand { Name = "can", Value = 8 },
                new HeatLand { Name = "ind", Value = 12 },
                new HeatLand { Name = "deu", Value = 13 },
                new HeatLand { Name = "chn", Value = 14 },
                new HeatLand { Name = "rus", Value = 11 },
                new HeatLand { Name = "fra", Value = 8 },
                new HeatLand { Name = "esp", Value = 7 },
                new HeatLand { Name = "kor", Value = 10 },
                new HeatLand { Name = "zaf", Value = 12 },
                new HeatLand { Name = "bra", Value = 13 },
                new HeatLand { Name = "are", Value = 13 }
            };
    }
}
