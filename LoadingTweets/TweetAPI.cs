using System.Collections.Generic;
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

        /// <summary>
        /// Searches tweets that contain keyword
        /// </summary>
        /// <param name="keyword">keyword that must contains in tweet</param>
        /// <param name="numberOfTweetsToLoad">maximum number of tweets that will be loaded</param>
        /// <returns>Collection of tweets</returns>
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
