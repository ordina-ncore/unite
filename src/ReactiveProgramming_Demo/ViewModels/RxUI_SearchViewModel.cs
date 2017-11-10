using ReactiveProgramming_Demo.Services;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace ReactiveProgramming_Demo.ViewModels
{
    public class RxUI_SearchViewModel : ReactiveObject
    {
        [ReactiveUI.Fody.Helpers.Reactive]
        public string SearchText { get; set; }

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
                    this.RaiseAndSetIfChanged(ref _SearchResults, value);
                });
            }
        }

        public RxUI_SearchViewModel()
        {
            IObservable<string> textChanging =
                this.WhenAnyValue(x => x.SearchText);

            var searchObserver = textChanging
                .Throttle(TimeSpan.FromMilliseconds(500))
                .DistinctUntilChanged()
                .Where(qry => !string.IsNullOrEmpty(qry))
                .Subscribe(
                    qry => SearchService.DoDummySearch(qry)
                    .ContinueWith(r => SearchResults = $"{DateTime.Now.ToString()}\n{r.Result}"));

        }
    }
}
