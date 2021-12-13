using System;

namespace ViewModelsSamples.General.Animations;

public class AvailableSpeed
{
    public AvailableSpeed(string name, TimeSpan speed)
    {
        Name = name;
        Speed = speed;
    }

    public string Name { get; set; }

    public TimeSpan Speed { get; set; }
}
