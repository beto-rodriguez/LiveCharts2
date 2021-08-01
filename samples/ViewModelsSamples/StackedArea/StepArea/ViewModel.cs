﻿using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.Generic;

namespace ViewModelsSamples.StackedArea.StepArea
{
    public class ViewModel
    {
        public List<ISeries> Series { get; set; } = new List<ISeries>
        {
            new StackedStepAreaSeries<double>
            {
                Values = new List<double> { 3, 2, 3, 5, 3, 4, 6 }
            },
            new StackedStepAreaSeries<double>
            {
                Values = new List<double> { 6, 5, 6, 3, 8, 5, 2 }
            },
            new StackedStepAreaSeries<double>
            {
                Values = new List<double> { 4, 8, 2, 8, 9, 5, 3 }
            }
        };
    }
}
