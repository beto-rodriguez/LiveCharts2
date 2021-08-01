using System.Collections.Generic;

namespace ViewModelsSamples.Maps.World
{
    public class ViewModel
    {
        // every country has a unique identifier
        // check the "shortName" property in the following
        // json file to assign a value to a country in the heat map
        // https://github.com/beto-rodriguez/LiveCharts2/blob/master/docs/_assets/word-map-index.json

        public Dictionary<string, double> Values { get; set; } = new Dictionary<string, double>
        {
            ["mex"] = 10,
            ["usa"] = 15,
            ["can"] = 8,
            ["ind"] = 12,
            ["deu"] = 13,
            ["chn"] = 14,
            ["rus"] = 11,
            ["fra"] = 8,
            ["esp"] = 7,
            ["kor"] = 10,
            ["zaf"] = 12,
            ["bra"] = 13,
            ["are"] = 13
        };
    }
}
