using LiveChartsCore;
using LiveChartsCore.Vortice;
using VorticeSample;

LiveCharts.Configure(config => config
    .AddDefaultMappers()
    .AddVortice()
    .AddVorticeDefaultTheme());

using TestApplication app = new();

var chart = new CartesianChart
{
    Series = [
        new ColumnSeries<double>
        {
            Values = [3, 2, 5, 3]
        }
    ]
};

app.AddControl(chart);

app.Run();
