using System;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.General.Animations;

public partial class ViewModel : ObservableObject
{
    private (string, Func<float, float>) _selectedCurve;
    private (string, TimeSpan) _selectedSpeed;

    public ViewModel()
    {
        _selectedCurve = AvalaibaleCurves[0];
        _selectedSpeed = AvailableSpeeds[1];
    }

    public ISeries[] Series { get; set; } =
        [new ColumnSeries<int> { Values = [5, 6, 3, 1, 8, 5, 3, 5, 6, 3, 1] }];

    public (string, Func<float, float>)[] AvalaibaleCurves =>
    [
        // LiveCharts already contains many common animating curves in the EasingFunctions static class.
        ("Back in", EasingFunctions.BackIn),
        ("Back out", EasingFunctions.BackOut),
        ("Back in out", EasingFunctions.BackInOut),
        ("Bounce in", EasingFunctions.BounceIn),
        ("Bounce out", EasingFunctions.BounceOut),
        ("Bounce in out", EasingFunctions.BounceInOut),
        ("Circle in", EasingFunctions.CircleIn),
        ("Circle out", EasingFunctions.CircleOut),
        ("Circle in out", EasingFunctions.CircleInOut),
        ("Cubic in", EasingFunctions.CubicIn),
        ("Cubic out", EasingFunctions.CubicOut),
        ("Cubic in out", EasingFunctions.CubicInOut),
        ("Ease", EasingFunctions.Ease),
        ("Ease in", EasingFunctions.EaseIn),
        ("Ease out", EasingFunctions.EaseOut),
        ("Ease in out", EasingFunctions.EaseInOut),
        ("Elastic in", EasingFunctions.ElasticIn),
        ("Elastic out", EasingFunctions.ElasticOut),
        ("Elastic in out", EasingFunctions.ElasticInOut),
        ("Exponential in", EasingFunctions.ExponentialIn),
        ("Exponential out", EasingFunctions.ExponentialOut),
        ("Exponential in out", EasingFunctions.ExponentialInOut),
        ("Lineal", EasingFunctions.Lineal),
        ("Polinominal in", EasingFunctions.PolinominalIn),
        ("Poliniminal out", EasingFunctions.PolinominalOut),
        ("Polinominal in out ", EasingFunctions.PolinominalInOut),
        ("Quadratic in", EasingFunctions.QuadraticIn),
        ("Quadratic out", EasingFunctions.QuadraticOut),
        ("Quadratic in out", EasingFunctions.QuadraticInOut),
        ("Sin in", EasingFunctions.SinIn),
        ("Sin out", EasingFunctions.SinOut),
        ("Sin in out", EasingFunctions.SinInOut),

        // the library also provides some common builders based on https://github.com/d3/d3-ease
        ("Custom back in", EasingFunctions.BuildCustomBackIn(2)),
        ("Custom elastic in", EasingFunctions.BuildCustomElasticIn(1.20f, 0.20f)),
        ("Custom back in", EasingFunctions.BuildCustomPolinominalIn(5)),

        // and also based on cubic bezier curves that are common in web development
        // you can build and play with custom cubic bezier curves at https://cubic-bezier.com/#.17,.67,.83,.67
        ("custom cubic bezier", EasingFunctions.BuildCubicBezier(0.17f, 0.67f, 0.83f, 0.67f)),
    ];

    public (string, TimeSpan)[] AvailableSpeeds =>
    [
        ("Slowest", TimeSpan.FromMilliseconds(1300)),
        ("Slow", TimeSpan.FromMilliseconds(800)),
        ("Medium", TimeSpan.FromMilliseconds(500)),
        ("Fast", TimeSpan.FromMilliseconds(300)),
        ("Fastest", TimeSpan.FromMilliseconds(100)),
    ];

    public (string, Func<float, float>) SelectedCurve
    {
        get => _selectedCurve;
        set
        {
            _selectedCurve = value;
            OnPropertyChanged(nameof(SelectedCurve));
            RestartAnimations();
        }
    }

    public (string, TimeSpan) SelectedSpeed
    {
        get => _selectedSpeed;
        set
        {
            _selectedSpeed = value;
            OnPropertyChanged(nameof(SelectedSpeed));
            RestartAnimations();
        }
    }

    private void RestartAnimations()
    {
        foreach (var series in Series) series.RestartAnimations();
    }
}
