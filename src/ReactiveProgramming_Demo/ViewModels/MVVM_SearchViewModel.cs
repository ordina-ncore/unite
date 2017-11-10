using ReactiveProgramming_Demo.Extensions;
using ReactiveProgramming_Demo.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace ReactiveProgramming_Demo.ViewModels
{
    public class MVVM_SearchViewModel : INotifyPropertyChanged
    {
        private string _SearchText;

        public string SearchText
        {
            get { return _SearchText; }
            set
            {
                _SearchText = value;
                OnPropertyChanged();
            }
        }

        string _SearchResults = "";
        public string SearchResults
        {
            get
            {
                return _SearchResults;
            }
            set
            {
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if(value != _SearchResults)
                    {
                        _SearchResults = value;
                        OnPropertyChanged();
                    }
                });
            }
        }

        public MVVM_SearchViewModel()
        {
            //var propertyChangedObservable =
            //    Observable.FromEventPattern<PropertyChangedEventHandler,
            //    PropertyChangedEventArgs>(
            //    x => PropertyChanged += x,
            //    x => PropertyChanged -= x);

            //var searchTextChangedObservable =
            //    propertyChangedObservable
            //    .Where(pc => pc.EventArgs.PropertyName == nameof(SearchText))
            //    .Select(_ => SearchText);

            //var searchObserver =
            //    searchTextChangedObservable
            //    .Throttle(TimeSpan.FromMilliseconds(500))
            //    .DistinctUntilChanged()
            //    .Subscribe(
            //        qry => SearchService.DoDummySearch(qry)
            //        .ContinueWith(r => SearchResults = $"{DateTime.Now.ToString()}\n{r.Result}"));


            var searchObserver =
                this.GetPropertyAsObservable(() => SearchText)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .DistinctUntilChanged()
                .Subscribe(
                    qry => SearchService.DoDummySearch(qry)
                    .ContinueWith(r => SearchResults = $"{DateTime.Now.ToString()}\n{r.Result}"));
        }

        public void OnPropertyChanged([CallerMemberName]string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
