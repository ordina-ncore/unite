namespace ReactiveProgramming_Demo.Models
{
    public class NewsItem : IDisplayable
    {
        public string City { get; }
        public string Title { get; }

        public NewsItem(string city, string title)
        {
            City = city;
            Title = title;
        }

        public string GetContent()
        {
            return Title;
        }
    }
}
