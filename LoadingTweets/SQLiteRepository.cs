using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoadingTweets.DbModels;

namespace LoadingTweets
{
    public class SQLiteRepository
    {
        SQLiteConnection _connection;

        public SQLiteRepository(bool overWrite = false)
        {
            CreateDbFileIfNotExists(overWrite);
            _connection = new SQLiteConnection(BuildConnectionString());
            GenerateTablesIfNotExists();
        }

        ~SQLiteRepository()
        {
            if (_connection.State == System.Data.ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        private void CreateDbFileIfNotExists(bool overWrite)
        {
            if (!overWrite)
            {
                if (!File.Exists(@".\SearchResults.sqlite"))
                {
                    SQLiteConnection.CreateFile("SearchResults.sqlite");
                }
            }
            else
            {
                SQLiteConnection.CreateFile("SearchResults.sqlite");
            }
        }
        private string BuildConnectionString()
        {
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = "SearchResults.sqlite";
            builder.ForeignKeys = true;
            builder.Version = 3;
            return builder.ToString();
        }
        private void GenerateTablesIfNotExists()
        {
            _connection.Open();
            using (SQLiteCommand cmd = new SQLiteCommand(_connection))
            {
                cmd.CommandText =
                    @"CREATE TABLE IF NOT EXISTS `Searches` ( 
                        `Id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 
                        `Date` TEXT NOT NULL, 
                        `Keyword` TEXT NOT NULL, 
                        `NumberOfTweets` INTEGER NOT NULL 
                      );
                      CREATE TABLE IF NOT EXISTS `Tweets` ( 
                        `Id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 
                        `UserName` TEXT NOT NULL, 
                        `Text` TEXT, 
                        `Date` TEXT NOT NULL,
                        `FavouriteCount` INTEGER NOT NULL, 
                        `SearchId` INTEGER, 
                        FOREIGN KEY(`SearchId`) REFERENCES `Searches`(`Id`) ON DELETE SET NULL 
                      );";
                cmd.ExecuteNonQuery();
            }
            _connection.Close();
        }

        public int InsertNewSearchItem(SearchModel item)
        {
            int insertedId = -1;
            _connection.Open();

            using (SQLiteCommand cmd = new SQLiteCommand(_connection))
            {
                cmd.CommandText =
                    $@"INSERT INTO `Searches` 
                        (`Date`, `Keyword`, `NumberOfTweets`) 
                       VALUES (@date, @keyword, @numberOfTweets);";
                cmd.Parameters.Add(new SQLiteParameter("@date", item.Date));
                cmd.Parameters.Add(new SQLiteParameter("@keyword", item.Keyword));
                cmd.Parameters.Add(new SQLiteParameter("@numberOfTweets", item.NumberOfTweets));
                cmd.ExecuteNonQuery();

                cmd.CommandText =
                    @"SELECT MAX(`Id`)
                      FROM `Searches`;";
                int.TryParse(cmd.ExecuteScalar().ToString(), out insertedId);
            }
            _connection.Close();
            return insertedId;
        }

        public void InsertManyNewTweetItems(IEnumerable<TweetModel> tweets)
        {
            _connection.Open();
            using (SQLiteCommand cmd = new SQLiteCommand(_connection))
            {
                cmd.CommandText = BuildMultiInsertionQuery(tweets);
                cmd.ExecuteNonQuery();
            }
            _connection.Close();
        }

        private string BuildMultiInsertionQuery(IEnumerable<TweetModel> tweets)
        {
            StringBuilder builder = new StringBuilder("INSERT INTO `Tweets` (`UserName`, `Text`, `Date`, `FavouriteCount`, `SearchId`) VALUES ");
            foreach (var tweet in tweets)
            {
                builder.Append($"('{ tweet.UserName.Replace('\'', '`') }', '{ tweet.Text.Replace('\'', '`') }', '{ tweet.Date }', { tweet.FavouriteCount }, { tweet.SearchId }), ");
            }
            builder.Remove(builder.Length - 2, 2);

            return builder.ToString();
        }
    }
}
