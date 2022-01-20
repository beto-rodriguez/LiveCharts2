﻿using Eto.Forms;

namespace EtoFormsSample.VisualTest.Tabs;

public class View : Panel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="View"/> class.
    /// </summary>
    public View()
    {
        InitializeComponent();
        Size = new Eto.Drawing.Size(50, 50);

        var tabsControl = new TabControl
        {
            Location = new Eto.Drawing.Point(0, 0),
            Size = new Eto.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        var tabPage1 = new TabPage("tab 1");
        tabPage1.Controls.Add(new Lines.Basic.View
        {
            Location = new Eto.Drawing.Point(0, 0),
            Size = new Eto.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        });

        var tabPage2 = new TabPage("tab 2");
        tabPage2.Controls.Add(new Bars.Basic.View
        {
            Location = new Eto.Drawing.Point(0, 0),
            Size = new Eto.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        });

        tabsControl.TabPages.AddRange(new[] { tabPage1, tabPage2 });
        Controls.Add(tabsControl);
    }
}