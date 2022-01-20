using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto.Forms;
using ViewModelsSamples.Maps.World;

namespace EtoFormsSample.Maps.World;

public class View : Panel
{
    public View()
    {
        var viewModel = new ViewModel();

        var chart = new GeoMap
        {
            Shapes = viewModel.Shapes,
            MapProjection = LiveChartsCore.Geo.MapProjection.Mercator,
        };

        Content = chart;
    }
}
