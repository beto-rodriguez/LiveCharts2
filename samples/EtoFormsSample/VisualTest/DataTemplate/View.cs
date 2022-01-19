using Eto.Forms;

namespace EtoFormsSample.VisualTest.DataTemplate;

public class View : Panel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="View"/> class.
    /// </summary>
    public View()
    {
        InitializeComponent();
        Size = new Eto.Drawing.Size(50, 50);

        var label = new Label
        {
            Text = "data templates are not supported in Eto.Forms...",

            // out of livecharts properties...
            Location = new Eto.Drawing.Point(0, 0),
            Size = new Eto.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };
        Controls.Add(label);
    }
}
