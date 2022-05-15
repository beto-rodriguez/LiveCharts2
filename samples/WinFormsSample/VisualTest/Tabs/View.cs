using System.Windows.Forms;

namespace WinFormsSample.VisualTest.Tabs;

public partial class View : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="View"/> class.
    /// </summary>
    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        var tabsControl = new TabControl
        {
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        var tabPage1 = new TabPage("tab 1");
        tabPage1.Controls.Add(new Lines.AutoUpdate.View
        {
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        });

        var tabPage2 = new TabPage("tab 2");
        tabPage2.Controls.Add(new Bars.AutoUpdate.View
        {
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        });

        tabsControl.TabPages.AddRange(new[] { tabPage1, tabPage2 });
        Controls.Add(tabsControl);
    }
}
