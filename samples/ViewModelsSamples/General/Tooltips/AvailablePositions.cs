using LiveChartsCore.Measure;

namespace ViewModelsSamples.General.Tooltips;

public class AvailablePositions(string name, TooltipPosition position)
{
    public string Name { get; set; } = name;
    public TooltipPosition Position { get; set; } = position;
}
