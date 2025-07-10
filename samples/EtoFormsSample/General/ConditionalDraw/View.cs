using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Eto;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.VisualStates;
using SkiaSharp;

namespace EtoFormsSample.General.ConditionalDraw;

public class View : Panel
{
    private readonly ObservableCollection<ObservableValue> _values;
    private readonly Random _random = new();

    public View()
    {
        _values =
        [
            new(2),
            new(3),
            new(4)
        ];

        var columnSeries = new ColumnSeries<ObservableValue>
        {
            Values = _values,
        };

        columnSeries.PointMeasured += OnPointMeasured;

        // define the danger state, a red fill.
        columnSeries.HasState("Danger", [
            (nameof(IDrawnElement.Fill), (object)new SolidColorPaint(SKColors.Red))
        ]);

        // the hover state is fired when the mouse is over the point
        columnSeries.HasState("Hover", new[]
        {
            (nameof(IDrawnElement.DropShadow), (object)new LvcDropShadow(4, 4, 16, 16, new(0, 0, 255)))
        });

        var cartesianChart = new CartesianChart
        {
            Series = [columnSeries],
            LegendPosition = LiveChartsCore.Measure.LegendPosition.Right,
        };

        Content = cartesianChart;
        _ = Randomize();
    }

    private void OnPointMeasured(ChartPoint point)
    {
        if (point.Context.DataSource is not ObservableValue observable) return;
        if (observable.Value > 5)
        {
            point.SetState("Danger");
        }
        else
        {
            point.ClearState("Danger");
        }
    }

    private async Task Randomize()
    {
        while (true)
        {
            await Task.Delay(3000);
            foreach (var item in _values)
            {
                item.Value = _random.Next(1, 10);
            }
        }
    }
}
