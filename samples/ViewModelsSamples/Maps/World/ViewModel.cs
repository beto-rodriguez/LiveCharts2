using System.Collections.Generic;

namespace ViewModelsSamples.Maps.World
{
    public class ViewModel
    {
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
