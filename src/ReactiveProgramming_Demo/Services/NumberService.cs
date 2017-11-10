using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ReactiveProgramming_Demo.Services
{
    public static class NumberService
    {
        internal static IObservable<int> GetNumbers()
        {
            return Observable.Create<int>(async observer =>
            {
                int i = 0;
                while (true)
                {
                    observer.OnNext(i);
                    i++;
                    await Task.Delay(500);
                }
            });
        }
    }
}
