using LiveChartsCore.SkiaSharpView.WinForms;
using System.Windows.Forms;
using ViewModelsSamples.Axes.NamedLabels;

namespace WinFormsSample.Axes.NamedLabels
{
    public partial class View : UserControl
    {
        private readonly CartesianChart cartesianChart;

        public View()
        {
            InitializeComponent();
            Size = new System.Drawing.Size(50, 50);

            var viewModel = new ViewModel();

            cartesianChart = new CartesianChart
            {
                Series = viewModel.Series,
                XAxes = viewModel.XAxes,
                YAxes = viewModel.YAxes,
                TooltipPosition = LiveChartsCore.Measure.TooltipPosition.Left,
                TooltipFont = new System.Drawing.Font("Courier New", 25),
                TooltipTextColor = System.Drawing.Color.FromArgb(255, 242, 244, 195),
                TooltipBackColor = System.Drawing.Color.FromArgb(255, 72, 0, 50),

                // out of livecharts properties...
                Location = new System.Drawing.Point(0, 0),
                Size = new System.Drawing.Size(50, 50),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
            };

            Controls.Add(cartesianChart);
        }
    }
}
