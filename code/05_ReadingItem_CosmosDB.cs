using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json.Linq;

namespace CosmosDB_viz_DynamoDB
{
    public static partial class CosmosDBvDynamoDB
    {
        /*--------------------------------------------------------------------------
         *                             ReadingMovie_async
         *--------------------------------------------------------------------------*/
        public static bool ReadingMovie_async_CosmosDB(int year, string title, bool report)
        {


            operationSucceeded = false;
            operationFailed = false;
            try
            {

                // Here we find the Andersen family via its LastName
                IQueryable<MovieModel> movieQuery = moviesContainer.GetItemLinqQueryable<MovieModel>(true)
                        .Where(f => f.Year == year && f.Title == title);

                // The query is executed synchronously here, but can also be executed asynchronously via the IDocumentQuery<T> interface
                Console.WriteLine("Running LINQ query...");

                foreach (MovieModel movie in movieQuery)
                {
                    movie_record_cosmosdb = movie;
                    Console.WriteLine("\tRead {0}", movie.Title);

                    if (report)
                        Console.WriteLine("  -- Reading the {0} movie \"{1}\" from the Movies table...", movie.Year, movie.Title);
                }
                if (movie_record_cosmosdb == null)
                {
                    if (report)
                        Console.WriteLine("     -- Sorry, that movie isn't in the Movies table.");
                    return (false);
                }
                else
                {
                    if (report)
                        Console.WriteLine("     -- Found it!  The movie record looks like this:\n" +
                                            movie_record_cosmosdb.Year + movie_record_cosmosdb.Title);
                    operationSucceeded = true;
                    return (true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("     FAILED to get the movie, because: {0}.", ex.Message);
                operationFailed = true;
            }
            return (false);
        }
        public static MovieModel ReadingMovieItem_async_CosmosDB(int year, string title)
        {
           return moviesContainer.GetItemLinqQueryable<MovieModel>(true)
                .Where(x => x.Year == year && x.Title== title)
                .AsEnumerable()
                .FirstOrDefault();
   
        }

        public static IEnumerable<MovieModel> ReadingMovieItem_async_List_CosmosDB(int year, string title)
        {
            return moviesContainer.GetItemLinqQueryable<MovieModel>(true)
     .Where(f => f.Year == 2018 && f.Title== "The Big New Movie")
                  .AsEnumerable(); }



        //public static FeedIterator<MovieModel> ReadingMovieItem_async_List_CosmosDB()
        //{
        //    return moviesContainer.GetItemQueryIterator<MovieModel>(
        //                "select c.Year, c.Title, c.info from c where Year=1998 AND (CONTAINS(Title,'B') OR CONTAINS(Title,'Hzz'))");
        //}

        public static FeedIterator<MovieModel> ReadingMovieItem_async_List_CosmosDB(string query)
        {
            return moviesContainer.GetItemQueryIterator<MovieModel>(
                       query);
        }
    }
}

