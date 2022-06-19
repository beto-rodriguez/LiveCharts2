﻿using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Bars.RowsWithLabels;

[ObservableObject]
public partial class ViewModel
{
    public ISeries[] Series { get; set; } =
    {
        new RowSeries<int>
        {
            Values = new List<int> { 8, -3, 4 },
            Stroke = null,
            DataLabelsPaint = new SolidColorPaint(new SKColor(45, 45, 45)),
            DataLabelsSize = 14,
            DataLabelsPosition = DataLabelsPosition.End
        },
        new RowSeries<int>
        {
            Values = new List<int> { 4, -6, 5 },
            Stroke = null,
            DataLabelsPaint = new SolidColorPaint(new SKColor(250, 250, 250)),
            DataLabelsSize = 14,
            DataLabelsPosition = DataLabelsPosition.Middle
        },
        new RowSeries<int>
        {
            Values = new List<int> { 6, -9, 3 },
            Stroke = null,
            DataLabelsPaint = new SolidColorPaint(new SKColor(45, 45, 45)),
            DataLabelsSize = 14,
            DataLabelsPosition = DataLabelsPosition.Start
        }
    };
}
