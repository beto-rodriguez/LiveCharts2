using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.Maps.World;

namespace EtoFormsSample.Maps.World;

public class View : Panel
{
    public View()
    {
        var viewModel = new ViewModel();

        var chart = new GeoMap
        {
            Series = viewModel.Series,
            MapProjection = LiveChartsCore.Geo.MapProjection.Mercator,
        };

        Content = chart;
    }
}
