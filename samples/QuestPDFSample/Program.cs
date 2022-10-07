﻿using System.Diagnostics;
using System.Reflection;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.SKCharts;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

Document.Create(container =>
{
    _ = container.Page(page =>
    {
        page.Size(PageSizes.A4);
        page.Margin(2, Unit.Centimetre);
        page.PageColor(Colors.White);
        page.DefaultTextStyle(x => x.FontSize(20));

        page.Header()
            .Text("Hello PDF!")
            .SemiBold().FontSize(36).FontColor(Colors.Blue.Medium);

        page.Content()
            .PaddingVertical(1, Unit.Centimetre)
            .Column(x =>
            {
                x.Spacing(20);
                x.Item().Text(Placeholders.LoremIpsum());

                x.Item()
                    .Canvas((canvas, size) =>
                    {
                        var cartesianChart = new SKCartesianChart
                        {
                            Width = (int)size.Width,
                            Height = (int)size.Height,
                            Series = new ISeries[]
                            {
                                new LineSeries<int> { Values = new int[] { 1, 5, 4, 6 } },
                                new ColumnSeries<int> { Values = new int[] { 4, 8, 2, 4 } }
                            }
                        };

                        cartesianChart.SaveImage(canvas);
                    });
            });

        page.Footer()
            .AlignCenter()
            .Text(x =>
            {
                x.Span("Page ");
                x.CurrentPageNumber();
            });
    });
})
.GeneratePdf("hello.pdf");

var path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!, "hello.pdf");
Console.WriteLine($"PDF generated at {path}");
Process.Start(new ProcessStartInfo { FileName = $"file:///{path}", UseShellExecute = true });
