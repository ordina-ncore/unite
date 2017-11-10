using ReactiveProgramming_Demo.Models;
using ReactiveProgramming_Demo.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace ReactiveProgramming_Demo.Views
{
    public sealed partial class Basic_WeatherObservablePage : Page, INotifyPropertyChanged
    {
        string _Output = "";
        public string Output
        {
            get
            {
                return _Output;
            }
            set
            {
                Set(ref _Output, value);
                _Output = value;
            }
        }

        public Basic_WeatherObservablePage()
        {
            InitializeComponent();

            var cities = new string[] { "Brussels", "Mechelen", "Hasselt" };

           IObservable<WeatherCondition> weatherFeed =
                WeatherService.GetCurrentConditions(cities);

            #region Observer
            var weatherObserver = weatherFeed
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(

                    wc => Output += $"{wc.City}: {wc.Text}\n",

                e => new MessageDialog(e.Message, "Oops, an error").ShowAsync(),
                () => new MessageDialog("Completed").ShowAsync());
            #endregion

            #region alternatives
            //LoadWeather(cities);
            //WeatherService.StreamWeatherConditions(cities, (wc) => Output += $"{wc.City}: {wc.Text}\n");
            #endregion
        }

        private async void LoadWeather(IEnumerable<string> cities)
        {
            try
            {
                while (true)
                {
                    foreach (var item in cities)
                    {
                        var condition = await WeatherService.GetWeatherCondition(item);
                        Output += $"{condition.City}: {condition.Text}\n";
                    }
                    await Task.Delay(2000);
                }
            }
            catch (Exception e)
            {
                new MessageDialog(e.Message, "Oops, an error").ShowAsync();
            }
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
