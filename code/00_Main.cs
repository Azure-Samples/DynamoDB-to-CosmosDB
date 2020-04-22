using System;
using System.Collections.Generic;
using System.Threading;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.Azure.Cosmos;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosDB_viz_DynamoDB
{
    public static partial class CosmosDBvDynamoDB
    {
        // Global variables
        public static bool operationSucceeded;
        public static bool operationFailed;
        public static AmazonDynamoDBClient client;
        public static CosmosClient client_cosmosDB;
        public static Database cosmosDatabase;
        public static Table moviesTable;
        public static Container moviesContainer;
        public static TableDescription moviesTableDescription;
        public static CancellationTokenSource source = new CancellationTokenSource();
        public static CancellationToken token = source.Token;
        public static Amazon.DynamoDBv2.DocumentModel.Document movie_record;
        public static MovieModel movie_record_cosmosdb;


        /*--------------------------------------------------------------------------
         *                Main
         *--------------------------------------------------------------------------*/
        static void Main(string[] args)
        {

            //  1.  Create a DynamoDB client connected to a DynamoDB-Local instance
            Console.WriteLine(stepString, 1,
              "Create a DynamoDB client connected to a DynamoDB-Local instance");
            if (!CreateClient(true) || !Pause())
                return;

            //  1a.  Create a CosmosDB client connected to a DynamoDB-Local instance
            if (!CreateClient_CosmosDB(true) || !Pause())
                return;

            //  2.  Create a DynamoDB table for movie data asynchronously
            Console.WriteLine(stepString, 2,
              "Create a table for movie data");
            CreatingTable_async(movies_table_name,
                                 movie_items_attributes,
                                 movies_key_schema,
                                 movies_table_provisioned_throughput).Wait();
            if (!Pause() || operationFailed)
                return;

            try { moviesTable = Table.LoadTable(CosmosDBvDynamoDB.client, movies_table_name); }
            catch (Exception ex)
            {
                operationFailed = true;
                Console.WriteLine(
                  " Error: Could not access the new '{0}' table after creating it;\n" +
                  "        Reason: {1}.", movies_table_name, ex.Message);
                Pause();
                return;
            }

            //  2a.  Create a Cosmosdb database and collection for movie data asynchronously
            CreatingTable_async_CosmosDB(
                movies_table_name,
                partition_key_name,
                movie_collection_provisioned_throughput).Wait();
            if (!Pause() || operationFailed)
                return;


            //  3.  Load movie data into the Movies table asynchronously into DynamoDB
            if ((moviesTableDescription != null) &&
                (moviesTableDescription.ItemCount == 0))
            {
                Console.WriteLine(stepString, 3,
                  "Load movie data into the Movies table");
                LoadingData_async(moviesTable, movieDataPath).Wait();
                if (!Pause() || operationFailed)
                    return;

                //  3a.  Load movie data into the Movies table asynchronously into CosmosDB
                LoadingData_async_CosmosDB(movieDataPath).Wait();
                if (!Pause() || operationFailed)
                    return;


            }
            else
            {
                Console.WriteLine(stepString, 3,
                  "Skipped: Movie data is already loaded in the Movies table");
                if (!Pause())
                    return;
            }



            Console.WriteLine(stepString, 4,
              "Add a new movie to the Movies table");
            //  4.  Add a new movie to the Movies table - DynamoDB
            Amazon.DynamoDBv2.DocumentModel.Document newItemDocument = new Amazon.DynamoDBv2.DocumentModel.Document
            {
                ["year"] = 2018,
                ["title"] = "The Big New Movie",
                ["info"] = Amazon.DynamoDBv2.DocumentModel.Document.FromJson(
          "{\"plot\" : \"Nothing happens at all.\",\"rating\" : 0}")
            };
            WritingNewMovie_async(newItemDocument).Wait();
            if (!Pause() || operationFailed)
                return;

            //  4a.  Add a new movie to the Movies table - CosmosDB
            MovieModel mv = new MovieModel()
            {
                Id = Guid.NewGuid().ToString(),
                Title = "The Big New Movie",
                Year = 2018,
                MovieInfo = new MovieInfo() { Plot = "Nothing happens at all.", Rating = 0 }
            };
            WritingNewMovie_async_cosmosDB(mv).Wait();
            if (!Pause() || operationFailed)
                return;


            Console.WriteLine(stepString, 5,
              "Read and display the new movie record that was just added");
            //  5.  Read and display the new movie record that was just added - DynamoDB
            ReadingMovie_async(2018, "The Big New Movie", true).Wait();
            if (!Pause() || operationFailed)
                return;

            //  5a.  Read and display the new movie record that was just added - CosmosDB
            ReadingMovie_async_CosmosDB(2018, "The Big New Movie", true);
            if (!Pause() || operationFailed)
                return;

            //  6.  Update the new movie record in various ways
            //-------------------------------------------------
            //  6a.  Create an UpdateItemRequest to:
            //       -- modify the plot and rating of the new movie, and
            //       -- add a list of actors to it
            Console.WriteLine(stepString, "6a",
              "Change the plot and rating for the new movie and add a list of actors");

            //DynamoDB
            UpdateItemRequest updateRequest = new UpdateItemRequest()
            {
                TableName = movies_table_name,
                Key = new Dictionary<string, AttributeValue>
                {
                    { partition_key_name, new AttributeValue { N = "2018" } },
                    { sort_key_name, new AttributeValue { S = "The Big New Movie" } }
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                  { ":r", new AttributeValue { N = "5.5" } },
                  { ":p", new AttributeValue { S = "Everything happens all at once!" } },
                  { ":a", new AttributeValue { L = new List<AttributeValue>
                    { new AttributeValue { S ="Larry" },
                        new AttributeValue { S = "Moe" },
                        new AttributeValue { S = "Curly" } }
                    }
                }
            },
                UpdateExpression = "SET info.rating = :r, info.plot = :p, info.actors = :a",
                ReturnValues = "NONE"
            };

            UpdatingMovie_async(updateRequest, true).Wait();
            if (!Pause() || operationFailed)
                return;

            //CosmosDB
            var doc = ReadingMovieItem_async_CosmosDB(2018, "The Big New Movie");
            MovieInfo info = new MovieInfo
            {
                Rating = 5.5,
                Plot = "Everything happens all at once!",
                Actors = new string[] { "Larry", "Moe", "Curly" }
            };
            doc.MovieInfo = info;
            UpdatingMovie_async_CosmosDB(doc, true).Wait();
            if (!Pause() || operationFailed)
                return;

            //  6b  Change the UpdateItemRequest so as to increment the rating of the
            //      new movie, and then make the update request asynchronously.
            Console.WriteLine(stepString, "6b",
            "Increment the new movie's rating atomically");

            Console.WriteLine("  -- Incrementing the rating of the new movie by 1...");
           
            //DynamoDB
            updateRequest.ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                 { ":inc", new AttributeValue { N = "1" } }
            };
            updateRequest.UpdateExpression = "SET info.rating = info.rating + :inc";
            UpdatingMovie_async(updateRequest, true).Wait();
            if (!Pause() || operationFailed)
                return;

            //CosmosDB
            var docList = ReadingMovieItem_async_List_CosmosDB(2018, "The Big New Movie");
            foreach (var docObject in docList)
            {
                info = docObject.MovieInfo;
                if (info != null)
                {
                    info.Rating++;
                    UpdatingMovie_async_CosmosDB(doc, true).Wait();
                    if (!Pause() || operationFailed)
                        return;
                }
            }

            //  6c  Change the UpdateItemRequest so as to increment the rating of the
            //      new movie, and then make the update request asynchronously.
            Console.WriteLine(stepString, "6c",
             "Now try the same increment again with a condition that fails... ");
            Console.WriteLine("  -- Now trying to increment the new movie's rating, but this time\n" +
                                "     ONLY ON THE CONDITION THAT the movie has more than 3 actors...");
            
            //DynamoDB
            updateRequest.ExpressionAttributeValues.Add(":n", new AttributeValue { N = "0" });

            updateRequest.ConditionExpression = "size(info.actors) > :n";
            UpdatingMovie_async(updateRequest, true).Wait();
            if (!Pause() || operationFailed)
                return;

            //CosmosDB
            FeedIterator<MovieModel> result = ReadingMovieItem_async_List_CosmosDB("select * from Movies c where c.year=2018 and c.title=\"The Big New Movie\" and ARRAY_LENGTH(c.info.actors)>1");
            List<Task> tasks = new List<Task>();
            while (result.HasMoreResults)
            {
                var resultModel = result.ReadNextAsync();
                resultModel.Wait();
                foreach (var movie in resultModel.Result.ToList<MovieModel>())
                {
                    movie.MovieInfo.Rating++;

                    tasks.Add(UpdatingMovie_async_CosmosDB(doc, true));
                    if (operationFailed)
                        return;
                }
            }
            Task.WhenAll(tasks).Wait();

            //  7.  Try conditionally deleting the movie that we added

            //  7a.  Try conditionally deleting the movie that we added
            Console.WriteLine(stepString, "7a",
        "Try deleting the new movie record with a condition that fails");
            Console.WriteLine("  -- Trying to delete the new movie,\n" +
                               "     -- but ONLY ON THE CONDITION THAT its rating is 8.0 or less...");

            //DynamoDB
            Expression condition = new Expression();
            condition.ExpressionAttributeValues[":val"] = 8.0;
            condition.ExpressionStatement = "info.rating <= :val";
            DeletingItem_async(moviesTable, 2018, "The Big New Movie", condition).Wait();
            if (!Pause() || operationFailed)
                return;

            //ComosDB
            DeletingItem_async_CosmosDB("select c from c join d in c.info where d.rating<8 AND c.year=2018 AND c.title='The Big New Movie'").GetAwaiter();
            if (!Pause() || operationFailed)
                return;

            //  7b.  Now increase the cutoff to 7.0 and try to delete again...
            Console.WriteLine(stepString, "7b",
              "Now increase the cutoff to 7.0 and try to delete the movie again...");
            Console.WriteLine("  -- Now trying to delete the new movie again,\n" +
                               "     -- but this time on the condition that its rating is 7.0 or less...");
            //DynamoDB
            condition.ExpressionAttributeValues[":val"] = 8.0;

            DeletingItem_async(moviesTable, 2018, "The Big New Movie", condition).Wait();
            if (!Pause() )//|| operationFailed)
                return;

            //CosmosDB
            DeletingItem_async_CosmosDB("select * from c where c.info.rating>7 AND c.year=2018 AND c.title='The Big New Movie'").GetAwaiter();

            //  8.  Query the Movies table in 3 different ways
            Search search;

            //  8a. Just query on the year
            Console.WriteLine(stepString, "8a",
              "Query the Movies table using a Search object for all movies from 1985");
            Console.WriteLine("  -- First, create a Search object...");
           
            //DynamoDB
            try { search = moviesTable.Query(1985, new Expression()); }
            catch (Exception ex)
            {
                Console.WriteLine("     ERROR: Failed to create the Search object because:\n            " +
                                   ex.Message);
                Pause();
                return;
            }
            Console.WriteLine("     -- Successfully created the Search object,\n" +
                               "        so now we'll display the movies retrieved by the query:");
            if ((search == null) || !Pause())
                return;
            SearchListing_async(search).Wait();
            if (!Pause() || operationFailed)
                return;

            //CosmosDB
            result = ReadingMovieItem_async_List_CosmosDB("select * from c where c.year=1985");
            while (result.HasMoreResults)
            {
                var resultModel = result.ReadNextAsync();
                resultModel.Wait();
                foreach (var movie in resultModel.Result.ToList<MovieModel>())
                {
                    Console.WriteLine(movie.PrintInfo());
                    if ( operationFailed)
                        return;
                }
            }



            //  8b. SearchListing_async
            Console.WriteLine(stepString, "8b",
        "Query for 1992 movies with titles from B... to Hzz... using Table.Query");
            Console.WriteLine("  -- Now setting up a QueryOperationConfig for the 'Search'...");
            
            //DynamoDB
            QueryOperationConfig config = new QueryOperationConfig
            {
                Filter = new QueryFilter()
            };
            config.Filter.AddCondition("year", QueryOperator.Equal, new DynamoDBEntry[] { 1992 });
            config.Filter.AddCondition("title", QueryOperator.Between, new DynamoDBEntry[] { "B", "Hzz" });
            config.AttributesToGet = new List<string> { "year", "title", "info" };
            config.Select = SelectValues.SpecificAttributes;
            Console.WriteLine("     -- Creating the Search object based on the QueryOperationConfig");
            try { search = moviesTable.Query(config); }
            catch (Exception ex)
            {
                Console.WriteLine("     ERROR: Failed to create the Search object because:\n            " +
                                   ex.Message);
                if (!Pause() || operationFailed)
                    return;
            }
            Console.WriteLine("     -- Successfully created the Search object,\n" +
                               "        so now we'll display the movies retrieved by the query.");
            if ((search == null) || !Pause())
                return;

            SearchListing_async(search).Wait();
            if (!Pause() || operationFailed)
                return;

            //CosmosDB
            result = ReadingMovieItem_async_List_CosmosDB("select c.year, c.title, c.info from c where c.year=1998 AND (CONTAINS(c.title,'B') OR CONTAINS(c.title,'Hzz'))");
            while (result.HasMoreResults)
            {
                var resultModel = result.ReadNextAsync();
                resultModel.Wait();
                foreach (var movie in resultModel.Result.ToList<MovieModel>())
                {
                    Console.WriteLine(movie.PrintInfo());
                    if ( operationFailed)
                        return;
                }
            }


            //  8c. Query using a QueryRequest
            Console.WriteLine(stepString, "8c",
        "Query the Movies table for 1992 movies with titles from M... to Tzz...");
            Console.WriteLine("  -- Next use a low-level query to retrieve a selection of movie attributes");

            //DynamoDB
            QueryRequest qRequest = new QueryRequest
            {
                TableName = "Movies",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#yr", "year" }
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                      { ":qYr",   new AttributeValue { N = "1992" } },
                      { ":tSt",   new AttributeValue { S = "M" } },
                      { ":tEn",   new AttributeValue { S = "Tzz" } }
                },
                KeyConditionExpression = "#yr = :qYr and title between :tSt and :tEn",
                ProjectionExpression = "#yr, title, info.actors[0], info.genres, info.running_time_secs"
            };
            Console.WriteLine("     -- Using a QueryRequest to get the lead actor and genres of\n" +
                               "        1992 movies with titles between 'M...' and 'Tzz...'.");
            ClientQuerying_async(qRequest).Wait();
            if (!Pause() || operationFailed)
                return;

            //CosmosDB
            result = ReadingMovieItem_async_List_CosmosDB("select c.year, c.title, c.info from c where c.year=1992 AND (CONTAINS(c.title,'M') OR CONTAINS(c.title,'Tzz'))");
            while (result.HasMoreResults)
            {
                var resultModel = result.ReadNextAsync().Result;
                foreach (var movie in resultModel.ToList<MovieModel>())
                {
                    Console.WriteLine(movie.PrintInfo());
                    if (  operationFailed)
                        return;
                }
            }

            //  9.  Try scanning the movies table to retrieve movies from several decades
            //  9a. Use Table.Scan with a Search object and a ScanFilter to retrieve movies from the 1950s
            Console.WriteLine(stepString, "9a",
        "Scan the Movies table to retrieve all movies from the 1950's");
            
            //DynamoDB
            ScanFilter filter = new ScanFilter();
            filter.AddCondition("year", ScanOperator.Between, new DynamoDBEntry[] { 1950, 1959 });
            ScanOperationConfig scanConfig = new ScanOperationConfig
            {
                Filter = filter
            };
            Console.WriteLine("     -- Creating a Search object based on a ScanFilter");
            try { search = moviesTable.Scan(scanConfig); }
            catch (Exception ex)
            {
                Console.WriteLine("     ERROR: Failed to create the Search object because:\n            " +
                                   ex.Message);
                Pause();
                return;
            }
            Console.WriteLine("     -- Successfully created the Search object");
            if ((search == null) || !Pause())
                return;

            SearchListing_async(search).Wait();
            if (!Pause() || operationFailed)
                return;

            //CosmosDB
            result = ReadingMovieItem_async_List_CosmosDB("select * from c where c.year BETWEEN 1950 AND 1959");
            while (result.HasMoreResults)
            {
                var resultModel = result.ReadNextAsync();
                resultModel.Wait();
                foreach (var movie in resultModel.Result.ToList<MovieModel>())
                {
                    Console.WriteLine(movie.PrintInfo());
                    if (operationFailed)
                        return;
                }
            }


            //  9b. Use AmazonDynamoDBClient.Scan to retrieve movies from the 1960s
            Console.WriteLine(stepString, "9b",
        "Use a low-level scan to retrieve all movies from the 1960's");
            Console.WriteLine("     -- Using a ScanRequest to get movies from between 1960 and 1969");

            //DynamoDB
            ScanRequest sRequest = new ScanRequest
            {
                TableName = "Movies",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                  { "#yr", "year" }
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":y_a", new AttributeValue { N = "1960" } },
                    { ":y_z", new AttributeValue { N = "1969" } },
                },
                FilterExpression = "#yr between :y_a and :y_z",
                ProjectionExpression = "#yr, title, info.actors[0], info.directors, info.running_time_secs"
            };

            ClientScanning_async(sRequest).Wait();
            if ( operationFailed)
                return;

            //CosmosDB
            result = ReadingMovieItem_async_List_CosmosDB("select c.title, c.year, c.info.actors[0], c.info.directors,c.info.running_time_secs from c where  c.year BETWEEN 1960 AND 1969");

            while (result.HasMoreResults)
            {
                var resultModel = result.ReadNextAsync();
                resultModel.Wait();
                foreach (var movie in resultModel.Result.ToList<MovieModel>())
                {
                    Console.WriteLine(movie.PrintInfo());
                    if ( operationFailed)
                        return;
                }
            }

            //  10.  Finally, delete the Movies table and all its contents
            Console.WriteLine(stepString, 10,
        "Finally, delete the Movies table and all its contents");
            
            //DynamoDB
            DeletingTable_async(movies_table_name).Wait();

            //CosmosDB
            DeletingCollection_async_CosmosDB().Wait();
            //For Cosmos DB Database needs to be delted for complete clean up
            DeletingDB_async_CosmosDB().Wait();
            // End:
            Console.WriteLine(
        "\n=================================================================================" +
        "\n            This concludes the DynamoDB viz-a-viz cosmodDB demo program" +
        "\n=================================================================================" +
        "\n                      ...Press any key to exit");
            Console.ReadKey();

            return;
        }


        /*--------------------------------------------------------------------------
         *          Pause
         *--------------------------------------------------------------------------*/
        static bool Pause()
        {
            if (operationFailed)
                Console.WriteLine("     Operation failed...");
            else if (operationSucceeded)
                Console.WriteLine("     Completed that step successfully!");
            Console.WriteLine("      ...Press [Esc] to exit, or any other key to continue");
            ConsoleKeyInfo keyInf = Console.ReadKey();
            Console.WriteLine();
            return (keyInf.Key != ConsoleKey.Escape);

        }
    }
}
