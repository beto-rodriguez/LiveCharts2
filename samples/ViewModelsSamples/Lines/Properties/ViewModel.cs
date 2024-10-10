using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Themes;
using SkiaSharp;

namespace ViewModelsSamples.Lines.Properties;

public partial class ViewModel : ObservableObject
{
    private readonly LvcColor[] _colors = ColorPalletes.FluentDesign;
    private readonly Random _random = new();
    private LineSeries<double> _lineSeries;
    private int _currentColor = 0;

    public ViewModel()
    {
        _lineSeries = new LineSeries<double>
        {
            Values = new List<double> { -2, -1, 3, 5, 3, 4, 6 },
            LineSmoothness = 0.5
        };

        _series = [_lineSeries];
    }

    [ObservableProperty]
    private ISeries[] _series;

    [RelayCommand]
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

    [RelayCommand]
    public void ChangeSeriesInstance()
    {
        _lineSeries = new LineSeries<double>
        {
            Values = new List<double> { -2, -1, 3, 5, 3, 4, 6 },
            LineSmoothness = 0.5
        };

        Series = [_lineSeries];
    }

    [RelayCommand]
    public void NewStroke()
    {
        var nextColorIndex = _currentColor++ % _colors.Length;
        var color = _colors[nextColorIndex];
        _lineSeries.Stroke = new SolidColorPaint(new SKColor(color.R, color.G, color.B)) { StrokeThickness = 3 };
    }

    [RelayCommand]
    public void NewFill()
    {
        var nextColorIndex = _currentColor++ % _colors.Length;
        var color = _colors[nextColorIndex];

        _lineSeries.Fill = new SolidColorPaint(new SKColor(color.R, color.G, color.B, 90));
    }

    [RelayCommand]
    public void NewGeometryFill()
    {
        var nextColorIndex = _currentColor++ % _colors.Length;
        var color = _colors[nextColorIndex];

        _lineSeries.GeometryFill = new SolidColorPaint(new SKColor(color.R, color.G, color.B));
    }

    [RelayCommand]
    public void NewGeometryStroke()
    {
        var nextColorIndex = _currentColor++ % _colors.Length;
        var color = _colors[nextColorIndex];

        _lineSeries.GeometryStroke = new SolidColorPaint(new SKColor(color.R, color.G, color.B)) { StrokeThickness = 3 };
    }

    [RelayCommand]
    public void IncreaseLineSmoothness()
    {
        if (_lineSeries.LineSmoothness == 1) return;

        _lineSeries.LineSmoothness += 0.1;
    }

    [RelayCommand]
    public void DecreaseLineSmoothness()
    {
        if (_lineSeries.LineSmoothness == 0) return;

        _lineSeries.LineSmoothness -= 0.1;
    }

    [RelayCommand]
    public void IncreaseGeometrySize()
    {
        if (_lineSeries.GeometrySize == 60) return;

        _lineSeries.GeometrySize += 10;
    }

    [RelayCommand]
    public void DecreaseGeometrySize()
    {
        if (_lineSeries.GeometrySize == 0) return;

        _lineSeries.GeometrySize -= 10;
    }
}
