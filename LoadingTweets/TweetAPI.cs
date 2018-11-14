using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace LoadingTweets
{
    public class TweetAPI
    {
        public TweetAPI(CredentialsConfig credentials)
        {
            Auth.ApplicationCredentials = new TwitterCredentials (credentials.ConsumerKey, credentials.ConsumerSecret, credentials.AccessToken, credentials.AccessTokenSecret);
        }


        public IEnumerable<ITweet> SearchTweetByKeyword(string keyword, int numberOfTweetsToLoad)
        {
            var tweetSearchOptions = new SearchTweetsParameters(keyword)
            {
                MaximumNumberOfResults = numberOfTweetsToLoad
            };
            return Search.SearchTweets(tweetSearchOptions);
        }
    }
}
