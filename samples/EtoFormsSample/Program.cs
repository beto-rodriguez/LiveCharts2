using Eto.Forms;
using System;

namespace EtoFormsSample;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        new Application(Eto.Platform.Detect).Run(new Form1());
    }
}
