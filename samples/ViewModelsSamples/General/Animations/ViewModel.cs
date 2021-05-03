using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.General.Animations
{
    public class ViewModel : INotifyPropertyChanged
    {
        private AvailableEasingCurve _selectedAvailableCurve;
        private Func<float, float> _actualCurve;
        private AvailableSpeed _selectedAvailableSpeed;
        private TimeSpan _actualSpeed;

        private ISeries[] _series = new ISeries[]
        {
            new ColumnSeries<int> { Values = new [] { 5, 6, 3, 1, 8, 5, 3, 5, 6, 3, 1} }
        };

        public ISeries[] Series { get => _series; set { _series = value; OnPropertyChanged(); } }

        public AvailableEasingCurve SelectedCurve
        {
            get => _selectedAvailableCurve;
            set
            {
                _selectedAvailableCurve = value;
                ActualCurve = _selectedAvailableCurve.EasingFunction;
                OnPropertyChanged();
            }
        }
        public Func<float, float> ActualCurve
        {
            get => _actualCurve;
            set
            {
                _actualCurve = value;
                RestartAnimations();
                OnPropertyChanged();
            }
        }

        public AvailableEasingCurve[] AvalaibaleCurves => new[]
        {
            // LiveCharts already contains many common animating curves in the EasingFunctions static class.
             new AvailableEasingCurve("Back in", EasingFunctions.BackIn),
             new AvailableEasingCurve("Back out", EasingFunctions.BackOut),
             new AvailableEasingCurve("Back in out", EasingFunctions.BackInOut),
             new AvailableEasingCurve("Bounce in", EasingFunctions.BounceIn),
             new AvailableEasingCurve("Bounce out", EasingFunctions.BounceOut),
             new AvailableEasingCurve("Bounce in out", EasingFunctions.BounceInOut),
             new AvailableEasingCurve("Circle in", EasingFunctions.CircleIn),
             new AvailableEasingCurve("Circle out", EasingFunctions.CircleOut),
             new AvailableEasingCurve("Circle in out", EasingFunctions.CircleInOut),
             new AvailableEasingCurve("Cubic in", EasingFunctions.CubicIn),
             new AvailableEasingCurve("Cubic out", EasingFunctions.CubicOut),
             new AvailableEasingCurve("Cubic in out", EasingFunctions.CubicInOut),
             new AvailableEasingCurve("Ease", EasingFunctions.Ease),
             new AvailableEasingCurve("Ease in", EasingFunctions.EaseIn),
             new AvailableEasingCurve("Ease out", EasingFunctions.EaseOut),
             new AvailableEasingCurve("Ease in out", EasingFunctions.EaseInOut),
             new AvailableEasingCurve("Elastic in", EasingFunctions.ElasticIn),
             new AvailableEasingCurve("Elastic out", EasingFunctions.ElasticOut),
             new AvailableEasingCurve("Elastic in out", EasingFunctions.ElasticInOut),
             new AvailableEasingCurve("Exponential in", EasingFunctions.ExponentialIn),
             new AvailableEasingCurve("Exponential out", EasingFunctions.ExponentialOut),
             new AvailableEasingCurve("Exponential in out", EasingFunctions.ExponentialInOut),
             new AvailableEasingCurve("Lineal", EasingFunctions.Lineal),
             new AvailableEasingCurve("Polinominal in", EasingFunctions.PolinominalIn),
             new AvailableEasingCurve("Poliniminal out", EasingFunctions.PolinominalOut),
             new AvailableEasingCurve("Polinominal in out ", EasingFunctions.PolinominalInOut),
             new AvailableEasingCurve("Quadratic in", EasingFunctions.QuadraticIn),
             new AvailableEasingCurve("Quadratic out", EasingFunctions.QuadraticOut),
             new AvailableEasingCurve("Quadratic in out", EasingFunctions.QuadraticInOut),
             new AvailableEasingCurve("Sin in", EasingFunctions.SinIn),
             new AvailableEasingCurve("Sin out", EasingFunctions.SinOut),
             new AvailableEasingCurve("Sin in out", EasingFunctions.SinInOut),

             // the library also provides some common builders based on https://github.com/d3/d3-ease
             new AvailableEasingCurve("Custom back in", EasingFunctions.BuildCustomBackIn(2)),
             new AvailableEasingCurve("Custom elastic in", EasingFunctions.BuildCustomElasticIn(1.20f, 0.20f)),
             new AvailableEasingCurve("Custom back in", EasingFunctions.BuildCustomPolinominalIn(5)),

             // and also based on cubic bezier curves that are common in web development
             // you can build and play with custom cubic bezier curves at https://cubic-bezier.com/#.17,.67,.83,.67
             new AvailableEasingCurve("custom cubic bezier", EasingFunctions.BuildCubicBezier(0.17f, 0.67f, 0.83f, 0.67f)),
        };

        public AvailableSpeed SelectedSpeed
        {
            get => _selectedAvailableSpeed;
            set
            {
                _selectedAvailableSpeed = value;
                ActualSpeed = _selectedAvailableSpeed.Speed;
                OnPropertyChanged();
            }
        }
        public TimeSpan ActualSpeed
        {
            get => _actualSpeed;
            set
            {
                _actualSpeed = value;
                RestartAnimations();
                OnPropertyChanged();
            }
        }

        public AvailableSpeed[] AvailableSpeeds => new[]
        {
            new AvailableSpeed("Slowest", TimeSpan.FromMilliseconds(1300)),
            new AvailableSpeed("Slow", TimeSpan.FromMilliseconds(800)),
            new AvailableSpeed("Medium", TimeSpan.FromMilliseconds(500)),
            new AvailableSpeed("Fast", TimeSpan.FromMilliseconds(300)),
            new AvailableSpeed("Fastest", TimeSpan.FromMilliseconds(100)),
        };

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RestartAnimations()
        {
            foreach (var series in Series)
                series.RestartAnimations();
        }
    }

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
}
