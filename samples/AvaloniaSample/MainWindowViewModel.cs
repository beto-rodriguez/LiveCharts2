using System.Collections.Generic;
using System.Linq;

namespace AvaloniaSample;

public class MainWindowViewModel
{
    public MainWindowViewModel()
    {
        Samples = ViewModelsSamples.Index.Samples.ToList();
    }

    public List<string> Samples { get; set; }
}
