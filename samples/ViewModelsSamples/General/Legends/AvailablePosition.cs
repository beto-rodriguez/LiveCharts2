using LiveChartsCore.Measure;

namespace ViewModelsSamples.General.Legends;

public class AvailablePosition(string name, LegendPosition position)
{
    public string Name { get; set; } = name;
    public LegendPosition Position { get; set; } = position;
}
