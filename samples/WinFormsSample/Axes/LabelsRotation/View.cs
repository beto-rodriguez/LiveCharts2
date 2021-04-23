using LiveChartsCore.SkiaSharpView.WinForms;
using System.Threading.Tasks;
using System.Windows.Forms;
using ViewModelsSamples.Axes.LabelsRotation;

namespace WinFormsSample.Axes.LabelsRotation
{
    public partial class View : UserControl
    {
        private readonly CartesianChart cartesianChart;
        private readonly ViewModel viewModel;
        private bool? isStreaming = false;

        public View()
        {
            InitializeComponent();
            Size = new System.Drawing.Size(100, 100);

            viewModel = new ViewModel();

            cartesianChart = new CartesianChart
            {
                Series = viewModel.Series,
                YAxes = viewModel.YAxes,

                // out of livecharts properties...
                Location = new System.Drawing.Point(0, 50),
                Size = new System.Drawing.Size(100, 50),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
            };

            Controls.Add(cartesianChart);

            var b1 = new TrackBar { Location = new System.Drawing.Point(0, 0), Width = 300, Minimum = -360, Maximum = 720 };
            b1.ValueChanged += (object sender, System.EventArgs e) =>
            {
                viewModel.YAxes[0].LabelsRotation = b1.Value;
            };
            Controls.Add(b1);

        }
    }
}
