using ReactiveProgramming_Demo.Services;
using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace ReactiveProgramming_Demo.Views
{
    public sealed partial class RX_SearchPage : Page, INotifyPropertyChanged
    {
        string _SearchResults = "";
        public string SearchResults
        {
            get
            {
                return _SearchResults;
            }
            set
            {
                Dispatcher.RunAsync(CoreDispatcherPriority.High, () => Set(ref _SearchResults, value));
            }
        }

        DispatcherTimer timer = null;

        public RX_SearchPage()
        {
            InitializeComponent();

            #region Reactive
            var textChangedObservable =
                Observable.FromEventPattern<TextChangedEventHandler, TextChangedEventArgs>(
                x => SearchTextBox.TextChanged += x,
                x => SearchTextBox.TextChanged -= x)
                .Select(args => (args.Sender as TextBox).Text);

            var textChangedSearchObserver =
                textChangedObservable
                .ObserveOn(SynchronizationContext.Current)
                .Throttle(TimeSpan.FromSeconds(.5))
                .DistinctUntilChanged()
                .Subscribe(
                    qry => SearchService.DoDummySearch(qry)
                    .ContinueWith(r => SearchResults = $"{DateTime.Now.ToString()}\n{r.Result}"));
            #endregion

            #region alternative
            SearchTextBox.TextChanged += SearchTextBox_TextChanged;
            #endregion

            #region alternativeWithTime
            // SearchTextBox.TextChanged += SearchTextBox_TextChangedWithTime;
            #endregion
        }

        private void SearchTextBox_TextChangedWithTime(object sender, TextChangedEventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }

            string query = (sender as TextBox).Text;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(.5);
            timer.Tick += async (_, __) =>
            {
                timer.Stop();
                var result = await SearchService.DoDummySearch(query);
                SearchResults = $"{DateTime.Now.ToString()}\n{result}";
            };
            timer.Start();
        }











        private async void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = (sender as TextBox).Text;
            var result = await SearchService.DoDummySearch(query);
            SearchResults = $"{DateTime.Now.ToString()}\n{result}";
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
