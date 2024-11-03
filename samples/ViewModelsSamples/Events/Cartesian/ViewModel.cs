using System.Collections.Generic;
using System.Diagnostics;
using LiveChartsCore;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Events.Cartesian;

public class ViewModel
{
    public ISeries[] Series { get; set; }

    public ViewModel()
    {
        var data = new Fruit[]
        {
            new() { Name = "Apple", Stock = 6 },
            new() { Name = "Orange", Stock = 4 },
            new() { Name = "Pinaple", Stock = 2 },
            new() { Name = "Potato", Stock = 4 },
            new() { Name = "Lettuce", Stock = 6 },
            new() { Name = "Cherry", Stock = 8 }
        };

        var series = new ColumnSeries<Fruit>
        {
            Values = data,
            YToolTipLabelFormatter = point => $"{point.Model?.Stock} {point.Model?.Name}",
            DataLabelsFormatter = point => $"{point.Model?.Stock} {point.Model?.Name}",
            DataLabelsPaint = new SolidColorPaint(new SKColor(30, 30, 30)),
            DataLabelsPosition = DataLabelsPosition.End,
            Mapping = (fruit, index) => new(index, fruit.Stock)
        };

        var selectedItems = new HashSet<Fruit>();

        _ = series
            .OnPointDown((chart, point) =>
            {
                if (!selectedItems.Contains(point.Model!))
                {
                    // if the item was not in the selectedItems hash set
                    // then we add the tapped fruit to the selectedItems collection
                    // and also set a red color on the drawn visual.
                    selectedItems.Add(point.Model);
                    point.Visual!.Fill = new SolidColorPaint(SKColors.Red);
                }
                else
                {
                    // if the element is already in the hash set, then we remove it
                    // finally we restore the original series fill color by setting the
                    // visual fill to null
                    selectedItems.Remove(point.Model);
                    point.Visual!.Fill = null;
                }

                Trace.WriteLine(
                    $"Clicked on {point.Model.Name}, {point.Model.Stock} items sold per day");

                //ensures the canvas is redrawn after we set the fill
                chart.Invalidate();
            })
            .OnPointHover((chart, point) =>
            {
                point.Visual!.Stroke = new SolidColorPaint(SKColors.Yellow);
                Trace.WriteLine($"  Hovered over {point.Model!.Name}");
                chart.Invalidate();
            })
            .OnPointHoverLost((chart, point) =>
            {
                point.Visual!.Stroke = null;
                Trace.WriteLine($"      Hover lost over {point.Model!.Name}");
                chart.Invalidate();
            });

        Series = [series];
    }
}
