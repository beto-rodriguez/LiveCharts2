using System;

namespace ViewModelsSamples.General.Animations
{
    public class AvailableEasingCurve
    {
        public AvailableEasingCurve(string name, Func<float, float> easingFunction)
        {
            Name = name;
            EasingFunction = easingFunction;
        }

        public string Name { get; set; }

        public Func<float, float> EasingFunction { get; set; }
    }
}
