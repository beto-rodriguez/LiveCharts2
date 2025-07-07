// based on CommunityToolkit:
// https://github.com/CommunityToolkit/WindowsCommunityToolkit/blob/main/Microsoft.Toolkit.Uwp.UI/Helpers/ThemeListener.cs

using System;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Windows.UI.ViewManagement;

namespace LiveChartsCore.SkiaSharpView.WinUI;

/// <summary>
/// Listens for changes to the Application Theme.
/// </summary>
public sealed class ThemeListener : IDisposable
{
    /// <summary>
    /// Gets or sets the Current Theme.
    /// </summary>
    public ApplicationTheme CurrentTheme { get; set; }

    /// <summary>
    /// Gets or sets which DispatcherQueue is used to dispatch UI updates.
    /// </summary>
    public DispatcherQueue DispatcherQueue { get; set; }

    /// <summary>
    /// An event that fires if the Theme changes.
    /// </summary>
    public event Action ThemeChanged;

    private readonly UISettings _settings = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeListener"/> class.
    /// </summary>
    /// <param name="action">The action to perform when the theme changes.</param>
    /// <param name="dispatcherQueue">The DispatcherQueue that should be used to dispatch UI updates, or null if this is being called from the UI thread.</param>
    public ThemeListener(Action action, DispatcherQueue? dispatcherQueue = null)
    {
        ThemeChanged += action;
        CurrentTheme = Application.Current.RequestedTheme;

        DispatcherQueue = dispatcherQueue ?? DispatcherQueue.GetForCurrentThread();

        _settings.ColorValuesChanged += Settings_ColorValuesChanged;

        // Fallback in case either of the above fail, we'll check when we get activated next.
        if (Window.Current is not null)
            Window.Current.CoreWindow.Activated += CoreWindow_Activated;
    }

    private void Settings_ColorValuesChanged(UISettings sender, object args) =>
        _ = DispatcherQueue.TryEnqueue(UpdateProperties);

    private void CoreWindow_Activated(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.WindowActivatedEventArgs args) =>
        UpdateProperties();

    /// <summary>
    /// Set our current properties and fire a change notification.
    /// </summary>
    private void UpdateProperties()
    {
        if (CurrentTheme == Application.Current.RequestedTheme) return;
        CurrentTheme = Application.Current.RequestedTheme;
        ThemeChanged?.Invoke();
    }

    /// <summary>
    /// Disposes of the ThemeListener.
    /// </summary>
    public void Dispose()
    {
        _settings.ColorValuesChanged -= Settings_ColorValuesChanged;
        if (Window.Current is not null)
            Window.Current.CoreWindow.Activated -= CoreWindow_Activated;
    }
}
