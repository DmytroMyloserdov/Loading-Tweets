﻿using LoadingTweets.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Models;

namespace LoadingTweets
{
    public class Command
    {
        public string Keyword { get; set; }
        public int NumberOfTweets { get; set; }

        /// <summary>
        /// Main function that call searching tweets and loading database
        /// </summary>
        public void ExecuteSearch()
        {
            var credentials = new CredentialsConfig();
            TweetAPI api = new TweetAPI(credentials);
            SQLiteRepository repo = new SQLiteRepository();

            do
            {
                Console.WriteLine("\nType command:");
                var command = Console.ReadLine().Trim();
                if (command == "exit")
                {
                    return;
                }

                try
                {
                    GetCommandContext(command);
                }
                catch (UnknownCommandException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }

                var tweets = api.SearchTweetByKeyword(Keyword, NumberOfTweets);
                var insertedSearchId = repo.InsertNewSearchItem(new SearchModel(DateTime.Now, Keyword, NumberOfTweets));

                repo.InsertManyNewTweetItems(TweetMap(tweets, insertedSearchId));
            }
            while (true);
        }

        /// <summary>
        /// Analyses command to get its data
        /// </summary>
        /// <param name="command">Command to be analysed</param>
        private void GetCommandContext(string command)
        {
            var context = command.Split(' ').Where(str => str != "").ToList();

            if (context[0].ToLower() != "search")
            {
                throw new UnknownCommandException($"Unknown command { context[0] }");
            }

            int kIndex = context.IndexOf("-k");
            if (kIndex == -1)
            {
                throw new UnknownCommandException("Not found flag '-k'");
            }

            int nIndex = context.IndexOf("-n");
            if (nIndex == -1)
            {
                throw new UnknownCommandException("Not found flag '-n'");
            }

            if (kIndex + 1 == nIndex)
            {
                throw new UnknownCommandException("Not fount value after '-k' flag");
            }

            if (nIndex + 1 == context.Count)
            {
                throw new UnknownCommandException("Not fount value after '-n' flag");
            }

            Keyword = context[kIndex + 1];
            int.TryParse(context[nIndex + 1], out int number);
            NumberOfTweets = number;
        }

        /// <summary>
        /// Mapps tweet from TweetAPI to database model
        /// </summary>
        /// <param name="tweets">Collection of tweets</param>
        /// <param name="searchId">id of search action</param>
        /// <returns>Collection of mapped tweets</returns>
        private IEnumerable<TweetModel> TweetMap(IEnumerable<ITweet> tweets, int searchId)
        {
            List<TweetModel> tweetModels = new List<TweetModel>();
            foreach(var tweet in tweets)
            {
                tweetModels.Add(new TweetModel(tweet.CreatedBy.Name, tweet.Text, tweet.CreatedAt.ToString(), tweet.FavoriteCount, searchId));
            }
            return tweetModels;
        }
    }

    public class UnknownCommandException : Exception
    {
        public UnknownCommandException(string mess) : base(mess) { }
    }
}
