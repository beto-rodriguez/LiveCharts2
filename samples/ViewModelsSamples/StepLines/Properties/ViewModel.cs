﻿using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Themes;
using SkiaSharp;

namespace ViewModelsSamples.StepLines.Properties;

[ObservableObject]
public partial class ViewModel
{
    private readonly LvcColor[] _colors = ColorPalletes.FluentDesign;
    private readonly Random _random = new();
    private StepLineSeries<double> _lineSeries;
    private int _currentColor = 0;

    public ViewModel()
    {
        _lineSeries = new StepLineSeries<double>
        {
            Values = new List<double> { -2, -1, 3, 5, 3, 4, 6 },
        };

        _series = new ISeries[] { _lineSeries };
    }

    [ObservableProperty]
    private ISeries[] _series;

    [ICommand]
    public void ChangeValuesInstance()
    {
        var t = 0;
        var values = new List<double>();
        for (var i = 0; i < 10; i++)
        {
            t += _random.Next(-5, 10);
            values.Add(t);
        }

        _lineSeries.Values = values;
    }

    public void ChangeSeriesInstance()
    {
        _lineSeries = new StepLineSeries<double>
        {
            Values = new List<double> { -2, -1, 3, 5, 3, 4, 6 },
        };

        Series = new ISeries[] { _lineSeries };
    }

    [ICommand]
    public void NewStroke()
    {
        var nextColorIndex = _currentColor++ % _colors.Length;
        var color = _colors[nextColorIndex];
        _lineSeries.Stroke = new SolidColorPaint(new SKColor(color.R, color.G, color.B)) { StrokeThickness = 3 };
    }

    [ICommand]
    public void NewFill()
    {
        var nextColorIndex = _currentColor++ % _colors.Length;
        var color = _colors[nextColorIndex];

        _lineSeries.Fill = new SolidColorPaint(new SKColor(color.R, color.G, color.B, 90));
    }

    [ICommand]
    public void NewGeometryFill()
    {
        var nextColorIndex = _currentColor++ % _colors.Length;
        var color = _colors[nextColorIndex];

        _lineSeries.GeometryFill = new SolidColorPaint(new SKColor(color.R, color.G, color.B));
    }

    [ICommand]
    public void NewGeometryStroke()
    {
        var nextColorIndex = _currentColor++ % _colors.Length;
        var color = _colors[nextColorIndex];

        _lineSeries.GeometryStroke = new SolidColorPaint(new SKColor(color.R, color.G, color.B)) { StrokeThickness = 3 };
    }

    [ICommand]
    public void IncreaseGeometrySize()
    {
        if (_lineSeries.GeometrySize == 60) return;

        _lineSeries.GeometrySize += 10;
    }

    [ICommand]
    public void DecreaseGeometrySize()
    {
        if (_lineSeries.GeometrySize == 0) return;

        _lineSeries.GeometrySize -= 10;
    }
}
