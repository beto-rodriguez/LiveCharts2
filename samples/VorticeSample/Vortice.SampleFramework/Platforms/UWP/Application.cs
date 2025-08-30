// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using Windows.ApplicationModel.Core;

namespace Vortice
{
    public abstract partial class Application : IDisposable
    {
        private void PlatformConstruct()
        {
            MainWindow = new Window("Vortice", 800, 600);
        }

        private void PlatformRun()
        {
            CoreApplication.Run(MainWindow);
        }

        private void PlatformExit()
        {
            CoreApplication.Exit();
        }
    }
}
