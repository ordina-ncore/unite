using ReactiveProgramming_Demo.ViewModels;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Windows.UI.Xaml.Controls;

namespace ReactiveProgramming_Demo.Views
{
    public sealed partial class RxUI_LoginPage : Page, INotifyPropertyChanged
    {
        public RxUI_LoginPage()
        {
            this.DataContext = new RxUI_LoginViewModel();
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
