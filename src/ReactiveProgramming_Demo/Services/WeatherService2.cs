using ReactiveProgramming_Demo.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ReactiveProgramming_Demo.Services
{
    public static class WeatherService2
    {
        internal static IObservable<WeatherCondition> GetCurrentConditions(IEnumerable<string> cities)
        {
            Debug.WriteLine("Observable created...");

            return Observable.Create<WeatherCondition>(observer =>
            {
                GetCurrentConditions(observer, cities);
                return () => { };
            });
        }

        private static async void GetCurrentConditions(IObserver<WeatherCondition> subscriber, IEnumerable<string> cities)
        {
            Debug.WriteLine("Processing...");
            while (true)
            {
                var observable = cities.ToObservable().Select(async c => await GetWeatherCondition(c));
                observable.Subscribe(async wc => subscriber.OnNext(await wc));
                await Task.Delay(2000);
            }
        }

            private static async Task<WeatherCondition> GetWeatherCondition(string city)
        {
            if (city == "Hasselt")
                await Task.Delay(5000);

            var client = new HttpClient();
            var result = await client.GetAsync($"https://query.yahooapis.com/v1/public/yql?q=select%20item.condition%20from%20weather.forecast%20where%20woeid%20in%20(select%20woeid%20from%20geo.places(1)%20where%20text=%22{city}%22)");

            var xml = await result.Content.ReadAsStringAsync();

            return new WeatherCondition(city, GetConditionFromXml(xml));
        }

        private static string GetConditionFromXml(string xml)
        {
            var xDoc = XDocument.Parse(xml);
            return xDoc.Root.Descendants("item").Descendants().FirstOrDefault().Attribute("text").Value;
        }
    }
}
