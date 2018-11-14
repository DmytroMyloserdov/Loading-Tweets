using System;

namespace LoadingTweets
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("USAGE");
            Console.WriteLine("search -k [keyword] -n [numberOfTweets]");
            Console.WriteLine("\t[keyword] - word that must contains in tweet");
            Console.WriteLine("\t[numberOfTweets] - maximum amount of tweets need to be loaded");
            Console.WriteLine();
            Console.WriteLine("To exit from program type 'exit'");
            Console.WriteLine();

            var command = new Command();
            command.ExecuteSearch();
        }
    }
}
