using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.Geo;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace ViewModelsSamples.General.ChartToImage
{
    public class ViewModel
    {
        public ISeries[] CatesianSeries { get; set; } = new ISeries[]
        {
            new LineSeries<int> { Values = new int[] { 1, 5, 4, 6 } },
            new ColumnSeries<int> { Values = new int[] { 4, 8, 2, 4 } }
        };

        public ISeries[] PieSeries { get; set; } = new ISeries[]
        {
            new PieSeries<int> { Values = new int[] { 10, } },
            new PieSeries<int> { Values = new int[] { 6 } },
            new PieSeries<int> { Values = new int[] { 4 } }
        };

        public IEnumerable<IMapElement> MapShapes { get; set; }
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
