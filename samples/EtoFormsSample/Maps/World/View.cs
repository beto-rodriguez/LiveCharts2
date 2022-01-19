using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto.Forms;
using ViewModelsSamples.Maps.World;

namespace EtoFormsSample.Maps.World;

public class View : Panel
{
    public View()
    {
        InitializeComponent();
        Size = new Eto.Drawing.Size(50, 50);

        var viewModel = new ViewModel();

        var chart = new GeoMap
        {
            Shapes = viewModel.Shapes,
            MapProjection = LiveChartsCore.Geo.MapProjection.Mercator,

            // out of livecharts properties...
            Location = new Eto.Drawing.Point(0, 0),
            Size = new Eto.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(chart);
    }
}
