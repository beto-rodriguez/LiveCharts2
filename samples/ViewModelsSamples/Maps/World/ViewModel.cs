using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LiveChartsCore.Geo;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace ViewModelsSamples.Maps.World
{
    public class ViewModel
    {
        private bool _isBrazilInChart = true;
        private readonly IWeigthedMapShape _brazil;
        private readonly Random _r = new Random();

        public ViewModel()
        {
            Series = new HeatLandSeries[]
            {
                new HeatLandSeries
                {
                    // every country has a unique identifier
                    // check the "shortName" property in the following
                    // json file to assign a value to a country in the heat map
                    // https://github.com/beto-rodriguez/LiveCharts2/blob/master/docs/_assets/word-map-index.json
                    Lands = new HeatLand[]
                    {
                        new HeatLand { Name = "bra", Value = 13 },
                        new HeatLand { Name = "mex", Value = 10 },
                        new HeatLand { Name = "usa", Value = 15 },
                        new HeatLand { Name = "can", Value = 8 },
                        new HeatLand { Name = "ind", Value = 12 },
                        new HeatLand { Name = "deu", Value = 13 },
                        new HeatLand { Name= "jpn", Value = 15 },
                        new HeatLand { Name = "chn", Value = 14 },
                        new HeatLand { Name = "rus", Value = 11 },
                        new HeatLand { Name = "fra", Value = 8 },
                        new HeatLand { Name = "esp", Value = 7 },
                        new HeatLand { Name = "kor", Value = 10 },
                        new HeatLand { Name = "zaf", Value = 12 },
                        new HeatLand { Name = "are", Value = 13 }
                    }
                }
            };

            _brazil = Series[0].Lands.First(x => x.Name == "bra");
            DoRandomChanges();
        }

        public HeatLandSeries[] Series { get; set; }

        public ICommand ToggleBrazilCommand => new Command(o => ToggleBrazil());

        private async void DoRandomChanges()
        {
            await Task.Delay(1000);

            while (true)
            {
                foreach (var shape in Series[0].Lands)
                {
                    shape.Value = _r.Next(0, 20);
                }

                await Task.Delay(500);
            }
        }

        private void ToggleBrazil()
        {
            if (_isBrazilInChart)
            {
                Series[0].Lands = Series[0].Lands.Where(x => x != _brazil).ToArray();
                _isBrazilInChart = false;
                return;
            }

            Series[0].Lands = Series[0].Lands.Concat(new[] { _brazil }).ToArray();
            _isBrazilInChart = true;
        }

        private void 
    }
}
