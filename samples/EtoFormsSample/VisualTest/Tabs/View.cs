using Eto.Forms;

namespace EtoFormsSample.VisualTest.Tabs;

public class View : Panel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="View"/> class.
    /// </summary>
    public View()
    {
        var tabsControl = new TabControl();

        var tabPage1 = new TabPage() { Text = "tab 1" };
        tabPage1.Content = new Lines.Basic.View();

        var tabPage2 = new TabPage() { Text = "tab 2" };
        tabPage2.Content = new Bars.Basic.View();

        tabsControl.Pages.Add(tabPage1);
        tabsControl.Pages.Add(tabPage2);

        Content = tabsControl;
    }
}
