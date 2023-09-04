using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Uno.Extensions.Reactive;

namespace UnoPlatformSample.Presentation;
public partial record MainModel
{
    public string? Title { get; }

    public IState<string> Name { get; }

    public MainModel(
        INavigator navigator,
        IStringLocalizer localizer)
    {
        _navigator = navigator;
        Title = $"Main - {localizer["ApplicationName"]}";
        Name = State<string>.Value(this, () => string.Empty);
    }

    public async Task GoToSecond()
    {
        var name = await Name;
        await _navigator.NavigateViewModelAsync<SecondModel>(this, data: new Entity(name!));
    }

    private INavigator _navigator;
}
