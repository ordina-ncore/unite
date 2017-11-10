using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ReactiveProgramming_Demo.Views
{
    public sealed partial class RX_AsyncPage : Page, INotifyPropertyChanged
    {
        public RX_AsyncPage()
        {
            InitializeComponent();

            var buttonObservable =
                Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
               x => btnLoad.Click += x,
               x => btnLoad.Click -= x).Subscribe(_ => DoWebRequest());
        }







        private async Task DoWebRequest()
        {
            var result = await GetWithRetriesOrReturnNull(
                $"https://query.yahooapis.com/v1/public/yql?q=select%20item.condition%20from%20weather.forecast%20where%20woeid%20in%20(select%20woeid%20from%20geo.places(1)%20where%20text=%22Hasselt%22)"
                , 2);

            new MessageDialog(result == null ? "Unable to do request" : "Success!")
                .ShowAsync();
        }







        public async Task<HttpResponseMessage> GetWithRetriesOrReturnNull(string url,
            int retries)
        {
            var result = Observable.FromAsync(() => DoWebRequest(url));
            return await result
                .Timeout(TimeSpan.FromMilliseconds(200))
                .Retry(retries)
                .OnErrorResumeNext(Observable.FromAsync(() => DoWebRequest2(url)))
                .Catch(Observable.Return<HttpResponseMessage>(null));
        }




        public async Task<HttpResponseMessage> DoWebRequest(string url)
        {
            Debug.WriteLine("Trying to call backend");
            if (new Random().Next(0, 5) <= 3)
            {
                Debug.WriteLine("Failed to connect to backend");
                throw new Exception("connection failed");
            }

            HttpClient client = new HttpClient();
            return await client.GetAsync(url);
        }

        public async Task<HttpResponseMessage> DoWebRequest2(string url)
        {
            //throw new Exception();
            HttpClient client = new HttpClient();
            return await client.GetAsync(url);
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
