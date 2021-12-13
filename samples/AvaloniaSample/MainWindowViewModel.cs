using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Media;
using LiveChartsCore;
using LiveChartsCore.Themes;
using LiveChartsCore.SkiaSharpView;

namespace AvaloniaSample;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private IBrush _background = new SolidColorBrush(new Color(120, 238, 238, 238));
    private IBrush _highlightedBackgound = new SolidColorBrush(new Color(255, 255, 255, 255));
    private IBrush _foreground = new SolidColorBrush(new Color(255, 112, 0, 0));

    public MainWindowViewModel()
    {
        Samples = ViewModelsSamples.Index.Samples.ToList();
        Samples.Insert(0, "Home");
    }

    public List<string> Samples { get; set; }

    public IBrush Background { get => _background; set { _background = value; OnPropertyChanged(); } }

    public IBrush HighlightedBackgound { get => _highlightedBackgound; set { _highlightedBackgound = value; OnPropertyChanged(); } }

    public IBrush Foreground { get => _foreground; set { _foreground = value; OnPropertyChanged(); } }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void SetLight()
    {
        Background = new SolidColorBrush(new Color(120, 238, 238, 238));
        HighlightedBackgound = new SolidColorBrush(new Color(255, 255, 255, 255));
        Foreground = new SolidColorBrush(new Color(255, 70, 70, 70));

        LiveCharts.Configure(
            settings => settings
                .AddDefaultMappers()
                .AddSkiaSharp()
                .AddLightTheme());
    }

    public void SetDark()
    {
        Background = new SolidColorBrush(new Color(120, 40, 40, 40));
        HighlightedBackgound = new SolidColorBrush(new Color(255, 40, 40, 40));
        Foreground = new SolidColorBrush(new Color(255, 250, 250, 250));

        LiveCharts.Configure(
            settings => settings
                .AddDefaultMappers()
                .AddSkiaSharp()
                .AddDarkTheme(
                    theme =>
                    {
                            // you can add additional rules to the current theme
                            theme.Style
                            .HasRuleForLineSeries(lineSeries =>
                            {
                                    // this method will be called in the constructor of a line series instance

                                    lineSeries.LineSmoothness = 0.65;
                                    // ...
                                    // add more custom styles here ...
                                }).HasRuleForBarSeries(barSeries =>
                            {
                                    // this method will be called in the constructor of a column series instance
                                    // ...
                                });
                    }));
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
