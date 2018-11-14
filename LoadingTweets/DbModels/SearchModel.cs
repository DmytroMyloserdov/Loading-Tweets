using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadingTweets.DbModels
{
    public class SearchModel
    {
        public string Date { get; set; }
        public string Keyword { get; set; }
        public int NumberOfTweets { get; set; }

        public SearchModel(DateTime date, string keyword, int numberOfTweets)
        {
            Date = date.ToString();
            Keyword = keyword;
            NumberOfTweets = numberOfTweets;
        }
    }
}
