﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadingTweets
{
    public class Command
    {
        public string Keyword { get; set; }
        public int NumberOfTweets { get; set; }


        public void ExecuteSearch()
        {
            var credentials = new CredentialsConfig();
            TweetAPI api = new TweetAPI(credentials);

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
                Console.WriteLine($"Found { tweets.Count() } tweets");
            }
            while (true);
        }

        void GetCommandContext(string command)
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

            Keyword = context[kIndex + 1];
            int.TryParse(context[nIndex + 1], out int number);
            NumberOfTweets = number;
        }
    }

    public class UnknownCommandException : Exception
    {
        public UnknownCommandException(string mess) : base(mess) { }
    }
}