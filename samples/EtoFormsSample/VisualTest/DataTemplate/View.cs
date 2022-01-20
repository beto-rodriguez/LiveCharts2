using Eto.Forms;

namespace EtoFormsSample.VisualTest.DataTemplate;

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
