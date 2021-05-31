using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WinForms;
using System;
using System.Windows.Forms;
using ViewModelsSamples.Bars.Basic;

namespace WinFormsSample.Bars.Basic
{
    public partial class View : UserControl
    {
        private readonly CartesianChart cartesianChart;

        /// <summary>
        /// Initializes a new instance of the <see cref="View"/> class.
        /// </summary>
        public View()
        {
            InitializeComponent();
            Size = new System.Drawing.Size(50, 50);

            var viewModel = new ViewModel();

            var cartesianChart = new CartesianChart
            {
                Series = new ISeries[] { new LineSeries<int> { Values = new[] { 2, 5, 4 } } },
                AnimationsSpeed = TimeSpan.FromMilliseconds(1000),

                // you can also pass your own easing function,
                // the EasingFunction property is of type Func<float, float>
                // this means that you can pass any function.
                // these functions take the time of the animation as parameter in percentage (from 0 to 1),
                // and returns the progress of the animation in percentage also (from 0 to 1).
                EasingFunction = (time) =>
                {
                    var progress = time * time; // quadratic curve

                    // here are some examples to define your own curves:

                    // a linear transition would be defined as follows:
                    // var progress = time;

                    // to use a sin out transition you could:
                    // var progress = (float)Math.Sin(time * Math.PI / 2d);

                    return progress;
                },

                // Or you can use the builders we provide
                // The builders LiveCharts provides are based on d3-ease library
                // you can learn more about these curves at:
                // https://github.com/d3/d3-ease
                // EasingFunction = LiveChartsCore.EasingFunctions.BuildCustomElasticOut(1.5f, 0.60f),



                // or try the ease out curve:
                // EasingFunction = LiveChartsCore.EasingFunctions.EaseOut,

                // out of livecharts properties...
                Location = new System.Drawing.Point(0, 0),
                Size = new System.Drawing.Size(50, 50),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
            };

            Controls.Add(cartesianChart);
        }
    }
}
