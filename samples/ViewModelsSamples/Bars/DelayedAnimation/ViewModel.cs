using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;

namespace ViewModelsSamples.Bars.DelayedAnimation;

public partial class ViewModel
{
    public float[] Values1 { get; set; } = FetchVales(0);
    public float[] Values2 { get; set; } = FetchVales(-0.15f);

    [RelayCommand]
    private void OnPointMeasured(ChartPoint point)
    {
        var baseAnimationsSpeed = 800f; // in milliseconds
        var perPointDelay = 100f; // in milliseconds
        var delay = point.Context.Entity.MetaData!.EntityIndex * perPointDelay;
        var speed = baseAnimationsSpeed + delay;
        var baseEasingFunction = EasingFunctions.BuildCustomElasticOut(1.5f, 0.60f);

        // the animation takes a function, that represents the progress of the animation
        // the parameter is the progress of the animation, it goes from 0 to 1
        // the function must return a value from 0 to 1, where 0 is the initial state
        // and 1 is the end state

        point.Context.Visual?.SetTransition(
            new Animation(progress =>
            {
                var d = delay / speed;

                return progress <= d
                    ? 0
                    : baseEasingFunction((progress - d) / (1 - d));
            },
            TimeSpan.FromMilliseconds(speed)));
    }

    private static float[] FetchVales(float offset)
    {
        var values = new List<float>();

        // the EasingFunctions.BounceInOut, is just
        // a function that looks nice!

        var fx = EasingFunctions.BounceInOut;
        var x = 0f;

        while (x <= 1)
        {
            values.Add(fx(x + offset));
            x += 0.025f;
        }

        return [.. values];
    }
}
