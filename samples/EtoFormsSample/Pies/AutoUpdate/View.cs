using System.Threading.Tasks;
using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto.Forms;
using ViewModelsSamples.Pies.AutoUpdate;

namespace EtoFormsSample.Pies.AutoUpdate;

public class View : Panel
{
    private readonly PieChart piechart;
    private readonly ViewModel viewModel;
    private bool? isStreaming = false;

    public View()
    {
        viewModel = new ViewModel();

        piechart = new PieChart
        {
            Series = viewModel.Series,
        };

        Content = piechart;

        var b1 = new Button { Text = "Add series" };
        b1.Click += (object sender, System.EventArgs e) => viewModel.AddSeries();

        var b2 = new Button { Text = "Remove series" };
        b2.Click += (object sender, System.EventArgs e) => viewModel.RemoveLastSeries();

        var b3 = new Button { Text = "Update all" };
        b3.Click += (object sender, System.EventArgs e) => viewModel.UpdateAll();

        var b4 = new Button { Text = "Constant changes" };
        b4.Click += OnConstantChangesClick;

        var layout = new DynamicLayout(
            new DynamicRow(b1, b2, b3, b4),
            new DynamicRow(piechart)
            );

        Content = layout;
    }

    private async void OnConstantChangesClick(object sender, System.EventArgs e)
    {
        isStreaming = isStreaming is null ? true : !isStreaming;

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
