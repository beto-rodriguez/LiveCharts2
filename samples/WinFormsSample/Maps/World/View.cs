﻿using LiveChartsCore.SkiaSharpView.WinForms;
using System.Windows.Forms;
using ViewModelsSamples.Maps.World;

namespace WinFormsSample.Maps.World
{
    public partial class View : UserControl
    {
        public View()
        {
            InitializeComponent();
            Size = new System.Drawing.Size(50, 50);

            var viewModel = new ViewModel();

            var chart = new GeoMap
            {
                Values = viewModel.Values,

                // out of livecharts properties...
                Location = new System.Drawing.Point(0, 0),
                Size = new System.Drawing.Size(50, 50),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
            };

            Controls.Add(chart);
        }
    }
}
