namespace UnoPlatform_v5.Presentation
{
    public sealed partial class Shell : UserControl, IContentControlProvider
    {
        public Shell()
        {
            this.InitializeComponent();
        }
        public ContentControl ContentControl => Splash;
    }
}