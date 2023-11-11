namespace UnoPlatform_v5.Presentation
{
    public class ShellModel
    {
        private readonly INavigator _navigator;

        public ShellModel(
            INavigator navigator)
        {
            _navigator = navigator;
            _ = Start();
        }

        public async Task Start()
        {
            await _navigator.NavigateViewModelAsync<MainModel>(this);
        }
    }
}