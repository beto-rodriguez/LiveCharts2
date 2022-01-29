using Eto.Forms;
using ViewModelsSamples.Test.MotionCanvasDispose;

namespace EtoFormsSample.Test.MotionCanvasDispose;

public class View : Panel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="View"/> class.
    /// </summary>
    public View()
    {
        var label = new Label
        {
            Text = "data templates are not supported in Eto.Forms...",
        };
        Content = label;
    }
}
