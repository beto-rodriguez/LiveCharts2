using System;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.Maui.Controls;

namespace MauiSample
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();

            Values = new Dictionary<string, double>
            {
                ["mex"] = 10,
                ["usa"] = 15,
                ["can"] = 8,
                ["ind"] = 12,
                ["deu"] = 13,
                ["chn"] = 14,
                ["rus"] = 11,
                ["fra"] = 8,
                ["esp"] = 7,
                ["kor"] = 10,
                ["zaf"] = 12,
                ["bra"] = 13,
                ["are"] = 13
            };

            BindingContext = this;
		}

        public Dictionary<string, double> Values { get; set; }
    }
}
