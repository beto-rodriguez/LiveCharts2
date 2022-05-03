using System;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using ViewModelsSamples.General.MultiThreading2;

namespace UWPSample.General.MultiThreading2
{
    public sealed partial class View : UserControl
    {
        public View()
        {
            InitializeComponent();

            var vm = new ViewModel(InvokeOnUIThread);
            DataContext = vm;
        }

        // this method takes another function as an argument.
        // the idea is that we are invoking the passed action in the UI thread
        // but the UI framework will let the view model how to do this.
        // we will pass the InvokeOnUIThread method to our view model so the view model knows how
        // to invoke an action in the UI thred.
        private void InvokeOnUIThread(Action action)
        {
            _ = CoreApplication.MainView.CoreWindow.Dispatcher
                .RunAsync(CoreDispatcherPriority.High, () => action());
        }
    }
}
