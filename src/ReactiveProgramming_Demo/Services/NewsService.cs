using ReactiveProgramming_Demo.Models;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ReactiveProgramming_Demo.Services
{
    public static class NewsService
    {
        internal static IObservable<NewsItem> GetNewItems(string city)
        {
            return Observable.Create<NewsItem>(async observer =>
            {
                int i = 0;
                while (i < 10)
                {
                    i++;
                    observer.OnNext(new NewsItem(city, "News article " + i));
                    await Task.Delay(1000);
                }
            });
        }
    }
}
