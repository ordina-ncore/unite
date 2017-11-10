namespace ReactiveProgramming_Demo.Models
{
    public class WeatherCondition : IDisplayable
    {
        public string City { get; }
        public string Text { get; }

        public WeatherCondition(string city, string text)
        {
            City = city;
            Text = text;
        }

        public string GetContent()
        {
            return Text;
        }
    }
}
