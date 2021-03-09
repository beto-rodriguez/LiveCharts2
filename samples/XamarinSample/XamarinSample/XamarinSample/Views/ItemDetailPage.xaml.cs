using System.ComponentModel;
using Xamarin.Forms;
using XamarinSample.ViewModels;

namespace XamarinSample.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}