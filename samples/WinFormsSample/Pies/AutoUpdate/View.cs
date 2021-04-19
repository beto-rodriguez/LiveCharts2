using LiveChartsCore.SkiaSharpView.WinForms;
using System.Threading.Tasks;
using System.Windows.Forms;
using ViewModelsSamples.Pies.AutoUpdate;

namespace WinFormsSample.Pies.AutoUpdate
{
    public partial class View : UserControl
    {
        private readonly PieChart piechart;
        private readonly ViewModel viewModel;
        private bool? isStreaming = false;

        public View()
        {
            InitializeComponent();
            Size = new System.Drawing.Size(100, 100);

            viewModel = new ViewModel();

            piechart = new PieChart
            {
                Series = viewModel.Series,

                // out of livecharts properties...
                Location = new System.Drawing.Point(0, 50),
                Size = new System.Drawing.Size(100, 50),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
            };

            Controls.Add(piechart);

            var b1 = new Button { Text = "Add series", Location = new System.Drawing.Point(0, 0) };
            b1.Click += (object sender, System.EventArgs e) => viewModel.AddSeries();
            Controls.Add(b1);

            var b2 = new Button { Text = "Remove series", Location = new System.Drawing.Point(80, 0) };
            b2.Click += (object sender, System.EventArgs e) => viewModel.RemoveLastSeries();
            Controls.Add(b2);

            var b3 = new Button { Text = "Update all", Location = new System.Drawing.Point(160, 0) };
            b3.Click += (object sender, System.EventArgs e) => viewModel.UpdateAll();
            Controls.Add(b3);

            var b4 = new Button { Text = "Constant changes", Location = new System.Drawing.Point(240, 0) };
            b4.Click += OnConstantChangesClick;
            Controls.Add(b4);
        }

        private async void OnConstantChangesClick(object sender, System.EventArgs e)
        {
            isStreaming = isStreaming == null ? true : !isStreaming;

            while (isStreaming.Value)
            {
                viewModel.RemoveLastSeries();
                viewModel.AddSeries();
                await Task.Delay(1000);
            }
        }

        private void B1_Click(object sender, System.EventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
