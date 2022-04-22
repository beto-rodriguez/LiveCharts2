using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace UnoSample
{
    [Activity(
            MainLauncher = true,
            ConfigurationChanges = global::Uno.UI.ActivityHelper.AllConfigChanges,
            WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.StateHidden
        )]
    public class MainActivity : Windows.UI.Xaml.ApplicationActivity
    {
    }
}

