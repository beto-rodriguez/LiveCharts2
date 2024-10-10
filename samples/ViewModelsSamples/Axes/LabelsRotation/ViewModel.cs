using System;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Axes.LabelsRotation;

public class ViewModel : ObservableObject
{
    private double _sliderValue = 15;

    public ISeries[] Series { get; set; } = [
        new LineSeries<double>
        {
            Values = [200, 558, 458, 249, 457, 339, 587 ],
            XToolTipLabelFormatter = (point) =>
                $"This is {Environment.NewLine}" +
                $"A multi-line label {Environment.NewLine}" +
                $"With a value of {Environment.NewLine}" + point.Coordinate.PrimaryValue,
        }
    ];

    public ICartesianAxis[] XAxes { get; set; } = [
        new Axis
        {
            // Use the Label property to indicate the format of the labels in the axis
            // The Labeler takes the value of the label as parameter and must return it as string
            Labeler = (value) =>
                $"This is {Environment.NewLine}" +
                $"A multi-line label {Environment.NewLine}" +
                $"With a value of {Environment.NewLine}" +value * 100,

            // The MinStep property lets you define the minimum separation (in chart values scale)
            // between every axis separator, in this case we don't want decimals,
            // so lets force it to be greater or equals than 1
            MinStep = 1,

            // labels rotations is in degrees (0 - 360)
            LabelsRotation = 45,

            SeparatorsPaint = new SolidColorPaint(SKColors.LightGray, 2)
        }
    ];

    public ICartesianAxis[] YAxes { get; set; } = [
        new Axis
        {
            LabelsRotation = 15,

            // Now the Y axis we will display it as currency
            // LiveCharts provides some common formatters
            // in this case we are using the currency formatter.
            Labeler = Labelers.Currency,

            // you could also build your own currency formatter
            // for example:
            // Labeler = (value) => value.ToString("C")

            // But the one that LiveCharts provides creates shorter labels when
            // the amount is in millions or trillions

            SeparatorsPaint = new SolidColorPaint(SKColors.LightGray, 2)
        }
    ];

    public double SliderValue
    {
        get => _sliderValue;
        set
        {
            if (SetProperty(ref _sliderValue, value))
            {
                YAxes[0].LabelsRotation = value;
            }
        }
    }
}
