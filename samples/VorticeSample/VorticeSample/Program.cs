using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.Vortice;
using VorticeSample;

LiveCharts.Configure(config => config
    .AddDefaultMappers()
    .AddVortice()
    .AddVorticeDefaultTheme()
    .HasRenderingSettings(renderSettings =>
    {
        renderSettings.ShowFPS = true;
    }));

using TestApplication app = new();

var data = new ObservableCollection<double> { 3, 2, 5, 3 };

var chart = new CartesianChart
{
    Series = [new ColumnSeries<double>(data)]
};

async Task randomize()
{
    var r = new Random();

    while (true)
    {
        await Task.Delay(2000);
        for (var i = 0; i < data.Count; i++)
            data[i] = r.NextDouble() * 10;
    }
}

_ = randomize();

app.AddControl(chart);

app.Run();
