using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CosmosDB_viz_DynamoDB
{
    public static partial class CosmosDBvDynamoDB
    {

        /*--------------------------------------------------------------------------
             *     LoadingData_async
             *--------------------------------------------------------------------------*/
        public static async Task LoadingData_async_CosmosDB(string filePath)
        {
            JArray movieArray;

            movieArray = await ReadJsonMovieFile_async_CosmosDB(filePath);
            if (movieArray != null)
                await LoadJsonMovieData_async_CosmosDB(movieArray);
        }

        /*--------------------------------------------------------------------------
         *                             ReadJsonMovieFile_async
         *--------------------------------------------------------------------------*/
        public static async Task<JArray> ReadJsonMovieFile_async_CosmosDB(string JsonMovieFilePath)
        {
            StreamReader sr = null;
            JsonTextReader jtr = null;
            JArray movieArray = null;

            Console.WriteLine("  -- Reading the movies data from a JSON file...");
            operationSucceeded = false;
            operationFailed = false;
            try
            {
                sr = new StreamReader(JsonMovieFilePath);
                jtr = new JsonTextReader(sr);
                movieArray = (JArray)await JToken.ReadFromAsync(jtr);
                operationSucceeded = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("     ERROR: could not read the file!\n          Reason: {0}.", ex.Message);
                operationFailed = true;
            }
            finally
            {
                if (jtr != null)
                    jtr.Close();
                if (sr != null)
                    sr.Close();
            }
            if (operationSucceeded)
            {
                Console.WriteLine("     -- Succeeded in reading the JSON file!");
                return (movieArray);
            }
            return (null);
        }


        /*--------------------------------------------------------------------------
         *                LoadJsonMovieData_async
         *--------------------------------------------------------------------------*/
        public static async Task LoadJsonMovieData_async_CosmosDB(JArray moviesArray)
        {
            operationSucceeded = false;
            operationFailed = false;

            client_cosmosDB.ClientOptions.AllowBulkExecution = true;

            cosmosDatabase = client_cosmosDB.GetDatabase(movies_table_name);
            moviesContainer = cosmosDatabase.GetContainer(movies_table_name);
            int n = moviesArray.Count;
            Console.Write("     -- Starting to load {0:#,##0} movie records into the Movies table asynchronously...\n" + "" +
              "        Wrote: ", n);
            List<Task> concurrentTasks = new List<Task>();
            for (int i = 0, j = 99; i < n; i++)
            {
                try
                {
                    MovieModel doc= JsonConvert.DeserializeObject<MovieModel>(moviesArray[i].ToString());
                    doc.Id = Guid.NewGuid().ToString();
                    concurrentTasks.Add(moviesContainer.CreateItemAsync(doc,new Microsoft.Azure.Cosmos.PartitionKey(doc.Year))); //moviesTable.PutItemAsync(doc);
                    if (i >= j)
                    {
                        j++;
                        Console.Write("{0,5:#,##0}, ", j);
                        if (j % 1000 == 0)
                            Console.Write("\n               ");
                        j += 99;
                    }
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\n     ERROR: Could not write the movie record #{0:#,##0}, because:\n       {1}",
                                       i, ex.Message);
                    operationFailed = true;
                    break;
                }
            }
            await Task.WhenAll(concurrentTasks);
            if (!operationFailed)
            {
                operationSucceeded = true;
                Console.WriteLine("\n     -- Finished writing all movie records to ComosDB!");
            }
        }

    }
}