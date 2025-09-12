namespace UnoPlatformSample.Presentation;

public class ShellModel
{
    private readonly INavigator _navigator;

    public ShellModel(
        INavigator navigator)
    {
        _navigator = navigator;
        // Add code here to initialize or attach event handlers to singleton services
    }
}
