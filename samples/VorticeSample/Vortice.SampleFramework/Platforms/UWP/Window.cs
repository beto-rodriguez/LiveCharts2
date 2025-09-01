// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System.Drawing;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;

namespace Vortice
{
    public sealed partial class Window : IFrameworkViewSource, IFrameworkView
    {
        private CoreWindow _coreWindow;
        private bool _activated;
        private Size _clientSize;

        public CoreApplicationView CoreApplicationView { get; internal set; }

        private void PlatformConstruct()
        {
        }

        IFrameworkView IFrameworkViewSource.CreateView()
        {
            return this;
        }

        void IFrameworkView.Initialize(CoreApplicationView applicationView)
        {
            CoreApplicationView = applicationView;
            applicationView.Activated += ApplicationView_Activated;
        }

        void IFrameworkView.SetWindow(CoreWindow window)
        {
            _coreWindow = window;
            UpdateSize(window);

            _coreWindow.SizeChanged += CoreWindow_SizeChanged;

            // Set handle.
            //OnHandleCreated(SwapChainHandle.CreateUWPCoreWindow(_coreWindow));
        }

        void IFrameworkView.Load(string entryPoint)
        {
        }

        void IFrameworkView.Run()
        {
            var applicationView = ApplicationView.GetForCurrentView();
            applicationView.Title = Title;
            _coreWindow.Activate();

            /*while (!_platform.ShouldExit)
            {
                _coreWindow.Dispatcher.ProcessEvents(CoreProcessEventsOption.ProcessAllIfPresent);

                // Tick
                if (_activated)
                {
                    _platform.Idle();
                }
            }*/
        }

        void IFrameworkView.Uninitialize()
        {
            // _platform.PostRun();
        }

        private void UpdateSize(CoreWindow window)
        {
            var bounds = window.Bounds;
            _clientSize = new Size((int)bounds.Width, (int)bounds.Height);
        }

        private void ApplicationView_Activated(CoreApplicationView sender, IActivatedEventArgs args)
        {
            CoreWindow.GetForCurrentThread().Activate();

            if (!_activated)
            {
                _activated = true;
            }
        }

        private void CoreWindow_SizeChanged(CoreWindow sender, WindowSizeChangedEventArgs args)
        {
            UpdateSize(sender);
        }

        private void SetTitle(string newTitle)
        {
            var applicationView = ApplicationView.GetForCurrentView();
            if (applicationView != null)
            {
                applicationView.Title = newTitle;
            }
        }
    }
}
