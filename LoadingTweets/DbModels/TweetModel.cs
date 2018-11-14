using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadingTweets.DbModels
{
    public class TweetModel
    {
        public string UserName { get; set; }
        public string Text { get; set; }
        public string Date { get; set; }
        public int FavouriteCount { get; set; }
        public int SearchId { get; set; }

        public TweetModel(string userName, string text, string date, int favouriteCount, int searchId)
        {
            UserName = userName;
            Text = text;
            Date = date;
            FavouriteCount = favouriteCount;
            SearchId = searchId;
        }
    }
}
