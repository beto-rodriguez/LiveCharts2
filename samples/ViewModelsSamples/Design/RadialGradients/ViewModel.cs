using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Design.RadialGradients
{
    public class ViewModel
    {
        public ViewModel()
        {
            // radial gradients are based on SkiaSharp circular gradients
            // for more info please see:
            // https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/effects/shaders/circular-gradients

            var colors = new[]
            {
                new SKColor(179, 229, 252),
                new SKColor(1, 87, 155)
                // ...

                // you can add as many colors as you require to build the gradient
                // by default all the distance between each colors is equal
                // use the colorPos parameter in the constructor of the RadialGradientPaintTask class
                // to specify the distance between each color
            };

            Series = new ISeries[]
            {
                new PieSeries<int>
                {
                    Values = new []{ 7 },
                    Stroke = null,
                    Fill = new RadialGradientPaintTask(colors),
                    MaxOuterRadius = 0.8
                },
                new PieSeries<int>
                {
                    Values = new []{ 3 },
                    Stroke = null,
                    Fill = new RadialGradientPaintTask(new SKColor(255, 205, 210), new SKColor(183, 28, 28))
                }
            };
        }

        public ISeries[] Series { get; set; }
    }
}
