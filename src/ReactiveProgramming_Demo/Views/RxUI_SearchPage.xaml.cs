using ReactiveProgramming_Demo.ViewModels;

using Windows.UI.Xaml.Controls;

namespace ReactiveProgramming_Demo.Views
{
    public sealed partial class RxUI_SearchPage : Page
    {
        public RxUI_SearchViewModel ViewModel { get; set; }

        public RxUI_SearchPage()
        {
            ViewModel = new RxUI_SearchViewModel();
            this.DataContext = ViewModel;
            InitializeComponent();
        }
    }
}
