using LiveChartsCore.Measure;

namespace ViewModelsSamples.General.Legends;

public class AvailablePosition
{
    public AvailablePosition(string name, LegendPosition position)
    {
        Name = name;
        Position = position;
    }

    public string Name { get; set; }
    public LegendPosition Position { get; set; }
}
