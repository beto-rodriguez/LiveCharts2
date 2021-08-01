﻿using LiveChartsCore.SkiaSharpView.WinForms;
using System.Windows.Forms;
using ViewModelsSamples.Lines.Zoom;

namespace WinFormsSample.Lines.Zoom
{
    public partial class View : UserControl
    {
        private readonly CartesianChart cartesianChart;

        public View()
        {
            InitializeComponent();
            Size = new System.Drawing.Size(50, 50);

            var viewModel = new ViewModel();

            cartesianChart = new CartesianChart
            {
                Series = viewModel.SeriesCollection,
                ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.X,

                // out of livecharts properties...
                Location = new System.Drawing.Point(0, 0),
                Size = new System.Drawing.Size(50, 50),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
            };

            Controls.Add(cartesianChart);
        }
    }
}
