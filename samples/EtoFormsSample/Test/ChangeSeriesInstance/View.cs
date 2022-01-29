using Eto.Forms;
using ViewModelsSamples.Test.ChangeSeriesInstance;

namespace EtoFormsSample.Test.ChangeSeriesInstance;

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
