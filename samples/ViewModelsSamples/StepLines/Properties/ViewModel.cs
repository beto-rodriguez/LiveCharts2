using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Themes;
using SkiaSharp;

namespace ViewModelsSamples.StepLines.Properties;

public class ViewModel : INotifyPropertyChanged
{
    private readonly LvcColor[] _colors = ColorPalletes.FluentDesign;
    private readonly Random _random = new();
    private StepLineSeries<double> _lineSeries;
    private int _currentColor = 0;
    private List<ISeries> _series;

    public ViewModel()
    {
        _lineSeries = new StepLineSeries<double>
        {
            Values = new List<double> { -2, -1, 3, 5, 3, 4, 6 },
        };

        _series = new List<ISeries>
        {
            _lineSeries
        };
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public List<ISeries> Series { get => _series; set { _series = value; OnPropertyChanged(); } }

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

        Series = new List<ISeries>
            {
                _lineSeries
            };
    }

    public void NewStroke()
    {
        var nextColorIndex = _currentColor++ % _colors.Length;
        var color = _colors[nextColorIndex];
        _lineSeries.Stroke = new SolidColorPaint(new SKColor(color.R, color.G, color.B)) { StrokeThickness = 3 };
    }

    public void NewFill()
    {
        var nextColorIndex = _currentColor++ % _colors.Length;
        var color = _colors[nextColorIndex];

        _lineSeries.Fill = new SolidColorPaint(new SKColor(color.R, color.G, color.B, 90));
    }

    public void NewGeometryFill()
    {
        var nextColorIndex = _currentColor++ % _colors.Length;
        var color = _colors[nextColorIndex];

        _lineSeries.GeometryFill = new SolidColorPaint(new SKColor(color.R, color.G, color.B));
    }

    public void NewGeometryStroke()
    {
        var nextColorIndex = _currentColor++ % _colors.Length;
        var color = _colors[nextColorIndex];

        _lineSeries.GeometryStroke = new SolidColorPaint(new SKColor(color.R, color.G, color.B)) { StrokeThickness = 3 };
    }

    public void IncreaseGeometrySize()
    {
        if (_lineSeries.GeometrySize == 60) return;

        _lineSeries.GeometrySize += 10;
    }

    public void DecreaseGeometrySize()
    {
        if (_lineSeries.GeometrySize == 0) return;

        _lineSeries.GeometrySize -= 10;
    }

    // The next commands are only to enable XAML bindings
    // they are not used in the WinForms sample
    public ICommand ChangeValuesInstanceCommand => new Command(o => ChangeValuesInstance());
    public ICommand ChangeSeriesInstanceCommand => new Command(o => ChangeSeriesInstance());
    public ICommand NewStrokeCommand => new Command(o => NewStroke());
    public ICommand NewFillCommand => new Command(o => NewFill());
    public ICommand NewGeometryFillCommand => new Command(o => NewGeometryFill());
    public ICommand NewGeometryStrokeCommand => new Command(o => NewGeometryStroke());
    public ICommand IncreaseGeometrySizeCommand => new Command(o => IncreaseGeometrySize());
    public ICommand DecreseGeometrySizeCommand => new Command(o => DecreaseGeometrySize());

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
