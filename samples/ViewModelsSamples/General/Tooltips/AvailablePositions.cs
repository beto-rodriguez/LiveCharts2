using LiveChartsCore.Measure;

namespace ViewModelsSamples.General.Tooltips;

public class AvailablePositions
{
    public AvailablePositions(string name, TooltipPosition position)
    {
        Name = name;
        Position = position;
    }

    public string Name { get; set; }
    public TooltipPosition Position { get; set; }
}
