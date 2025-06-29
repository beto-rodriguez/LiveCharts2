using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WinForms;
using SkiaSharp;
using System.Threading.Tasks;
using LiveChartsCore.VisualStates;
using LiveChartsCore.Drawing;

#pragma warning disable IDE0058 // Expression value is never used

namespace WinFormsSample.General.ConditionalDraw;

public partial class View : UserControl
{
    private readonly ObservableCollection<ObservableValue> _values;
    private readonly Random _random = new();

    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(400, 400);

        _values = [
            new(2),
            new(3),
            new(4)
        ];

        var columnSeries = new ColumnSeries<ObservableValue>
        {
            Values = _values,
        };

        columnSeries.PointMeasured += OnPointMeasured;

        // When the state is "Danger", the Fill is set to red
        columnSeries.HasState("Danger", [
            (nameof(IDrawnElement.Fill), new SolidColorPaint(SKColors.Red))
        ]);

        // The "Hover" state is fired when the pointer is over a point
        columnSeries.HasState("Hover", [
            (nameof(IDrawnElement.DropShadow), new LvcDropShadow(4, 4, 16, 16, new(0, 0, 255)))
        ]);

        var cartesianChart = new CartesianChart
        {
            Series = [columnSeries],
            LegendPosition = LiveChartsCore.Measure.LegendPosition.Right,
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(400, 400),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(cartesianChart);

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
