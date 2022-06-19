﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.WinForms;
using ViewModelsSamples.Events.Cartesian;

namespace WinFormsSample.Events.Cartesian;

public partial class View : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="View"/> class.
    /// </summary>
    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        var viewModel = new ViewModel();

        var cartesianChart = new CartesianChart
        {
            Series = viewModel.Series,

            // out of livecharts properties...
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(cartesianChart);
    }
}
