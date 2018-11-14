using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace LoadingTweets
{
    public class CredentialsConfig
    {
        public string ConsumerKey { get; } 
        public string ConsumerSecret { get; }
        public string AccessToken { get; }
        public string AccessTokenSecret { get; }

        public CredentialsConfig()
        {
            ConsumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
            ConsumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];
            AccessToken = ConfigurationManager.AppSettings["AccessToken"];
            AccessTokenSecret = ConfigurationManager.AppSettings["AccessTokenSecret"];
        }
    }
}
