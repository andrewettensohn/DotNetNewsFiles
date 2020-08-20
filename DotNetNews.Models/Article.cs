using SQLite;
using System.Collections.Generic;

namespace DotNetNews.Models
{
    public class Article
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string By { get; set; }

        public int Descendants { get; set; }
        [Ignore]
        public List<int> Kids { get; set; }

        public int Score { get; set; }

        public int Time { get; set; }

        public string Title { get; set; }

        public string Type { get; set; }

        public string Url { get; set; }

        public bool Saved { get; set; }

    }
}
