using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.Kernel;

namespace ViewModelsSamples.General.VisualElements;

public class ViewModel
{
    public IEnumerable<ChartElement> VisualElements { get; set; }

    public ISeries[] Series { get; set; }

    public ViewModel()
    {
        VisualElements = [
            new RectangleVisual(),
            new ScaledRectangleVisual(),
            new PointerDownAwareVisual(),
            new SvgVisual(),
            new CustomVisual(),
            new AbsoluteVisual(),
            new StackedVisual(),
            new TableVisual(),
            new ContainerVisual(),
        ];

        // no series are needed for this example
        Series = [];
    }
}
