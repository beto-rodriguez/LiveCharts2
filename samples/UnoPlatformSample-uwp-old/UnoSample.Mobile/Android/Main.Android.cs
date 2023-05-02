using Android.Runtime;
using Com.Nostra13.Universalimageloader.Core;
using System;
using Windows.UI.Xaml.Media;

namespace UnoSample.Droid
{
    [global::Android.App.ApplicationAttribute(
        Label = "@string/ApplicationName",
        Icon = "@mipmap/icon",
        LargeHeap = true,
        HardwareAccelerated = true,
        Theme = "@style/AppTheme"
    )]
    public class Application : Windows.UI.Xaml.NativeApplication
    {
        public Application(IntPtr javaReference, JniHandleOwnership transfer)
            : base(() => new App(), javaReference, transfer)
        {
            ConfigureUniversalImageLoader();
        }

        private static void ConfigureUniversalImageLoader()
        {
            // Create global configuration and initialize ImageLoader with this config
            var config = new ImageLoaderConfiguration
                .Builder(Context)
                .Build();

            ImageLoader.Instance.Init(config);

            ImageSource.DefaultImageLoader = ImageLoader.Instance.LoadImageAsync;
        }
    }
}
