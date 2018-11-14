using LoadingTweets.DbModels;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace LoadingTweets
{
    public class SQLiteRepository
    {
        SQLiteConnection _connection;

        /// <summary>
        /// Creates instance, creates connection and database file
        /// </summary>
        /// <param name="overWrite">Need to overwrite database</param>
        public SQLiteRepository(bool overWrite = false)
        {
            CreateDbFileIfNotExists(overWrite);
            _connection = new SQLiteConnection(BuildConnectionString());
            GenerateTablesIfNotExists();
        }

        /// <summary>
        /// Generates database file and overwrite it if need it
        /// </summary>
        /// <param name="overWrite">Need to overwrite database</param>
        private void CreateDbFileIfNotExists(bool overwrite)
        {
            if (!overwrite)
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

        /// <summary>
        /// Builds connsection to database with search results
        /// </summary>
        /// <returns>Connection string</returns>
        private string BuildConnectionString()
        {
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = "SearchResults.sqlite";
            builder.ForeignKeys = true;
            builder.Version = 3;
            return builder.ToString();
        }

        /// <summary>
        /// Query for creating tables in database if they are not exist
        /// </summary>
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

        /// <summary>
        /// Inserts search action into database
        /// </summary>
        /// <param name="item">Database model af search action</param>
        /// <returns>Id of inserted search action</returns>
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

        /// <summary>
        /// Inserts into database tweets
        /// </summary>
        /// <param name="tweets">Collections of mapped tweets</param>
        public void InsertManyNewTweetItems(IEnumerable<TweetModel> tweets)
        {
            _connection.Open();
            using (SQLiteCommand cmd = new SQLiteCommand(_connection))
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                using (var transaction = _connection.BeginTransaction())
                {
                    foreach (var tweet in tweets)
                    {
                        cmd.CommandText =
                            $@"INSERT INTO `Tweets` 
                            (`UserName`, `Text`, `Date`, `FavouriteCount`, `SearchId`) 
                           VALUES 
                            (@userName, @text, @date, @favouriteCount, @searchId)";
                        cmd.Parameters.Add(new SQLiteParameter("@userName", tweet.UserName.Replace('\'', '`')));
                        cmd.Parameters.Add(new SQLiteParameter("@text", tweet.Text.Replace('\'', '`')));
                        cmd.Parameters.Add(new SQLiteParameter("@date", tweet.Date));
                        cmd.Parameters.Add(new SQLiteParameter("@favouriteCount", tweet.FavouriteCount));
                        cmd.Parameters.Add(new SQLiteParameter("@searchId", tweet.SearchId));
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                sw.Stop();
                Console.WriteLine(sw.Elapsed.TotalSeconds);
            }
            _connection.Close();
        }
    }
}
