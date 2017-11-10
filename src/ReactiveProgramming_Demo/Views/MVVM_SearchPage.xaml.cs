using ReactiveProgramming_Demo.ViewModels;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Windows.UI.Xaml.Controls;

namespace ReactiveProgramming_Demo.Views
{
    public sealed partial class MVVM_SearchPage : Page
    {
        public MVVM_SearchViewModel ViewModel { get; set; }

        public MVVM_SearchPage()
        {
            ViewModel = new MVVM_SearchViewModel();
            this.DataContext = ViewModel;
            InitializeComponent();
        }
    }
}
