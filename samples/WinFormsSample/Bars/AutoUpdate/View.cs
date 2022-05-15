using System.Threading.Tasks;
using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.WinForms;
using ViewModelsSamples.Bars.AutoUpdate;

namespace WinFormsSample.Bars.AutoUpdate;

public partial class View : UserControl
{
    private readonly CartesianChart _cartesianChart;
    private readonly ViewModel _viewModel;
    private bool? _isStreaming = false;

    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(100, 100);

        _viewModel = new ViewModel();

        _cartesianChart = new CartesianChart
        {
            Series = _viewModel.Series,

            // out of livecharts properties...
            Location = new System.Drawing.Point(0, 50),
            Size = new System.Drawing.Size(100, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(_cartesianChart);

        var b1 = new Button { Text = "Add item", Location = new System.Drawing.Point(0, 0) };
        b1.Click += (object sender, System.EventArgs e) => _viewModel.AddItem();
        Controls.Add(b1);

        var b2 = new Button { Text = "Replace item", Location = new System.Drawing.Point(80, 0) };
        b2.Click += (object sender, System.EventArgs e) => _viewModel.ReplaceItem();
        Controls.Add(b2);

        var b3 = new Button { Text = "Remove item", Location = new System.Drawing.Point(160, 0) };
        b3.Click += (object sender, System.EventArgs e) => _viewModel.RemoveItem();
        Controls.Add(b3);

        var b4 = new Button { Text = "Add series", Location = new System.Drawing.Point(240, 0) };
        b4.Click += (object sender, System.EventArgs e) => _viewModel.AddSeries();
        Controls.Add(b4);

        var b5 = new Button { Text = "Remove series", Location = new System.Drawing.Point(320, 0) };
        b5.Click += (object sender, System.EventArgs e) => _viewModel.RemoveSeries();
        Controls.Add(b5);

        var b6 = new Button { Text = "Constant changes", Location = new System.Drawing.Point(400, 0) };
        b6.Click += OnConstantChangesClick;
        Controls.Add(b6);
    }

    private async void OnConstantChangesClick(object sender, System.EventArgs e)
    {
        _isStreaming = _isStreaming is null ? true : !_isStreaming;

        while (_isStreaming.Value)
        {
            _viewModel.RemoveItem();
            _viewModel.AddItem();
            await Task.Delay(1000);
        }
    }

    private void B1_Click(object sender, System.EventArgs e)
    {
        throw new System.NotImplementedException();
    }
}
