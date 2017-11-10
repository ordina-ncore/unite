using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ReactiveProgramming_Demo.Services
{
    public static class SearchService
    {
        public static async Task<string> DoDummySearch(string input)
        {
            Debug.WriteLine($"Search fired for {input}...");
            Random random = new Random();
            int randomNumber = random.Next(500, 2000);
            await Task.Delay(randomNumber);
            Debug.WriteLine($"Search for {input} done!");
            return $"these are the results for {input}...";
        }
    }
}
