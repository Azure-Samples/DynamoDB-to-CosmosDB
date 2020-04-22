using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
//using Amazon.DynamoDBv2.Model;

namespace CosmosDB_viz_DynamoDB
{
    public static partial class CosmosDBvDynamoDB
    {
        /*--------------------------------------------------------------------------
         *                       CreatingTable_async
         *--------------------------------------------------------------------------*/
        public static async Task CreatingTable_async_CosmosDB(string new_collection_name,
                                   string partitionKey,
                                   int provisionedThroughput)
        {
             cosmosDatabase = await client_cosmosDB.CreateDatabaseIfNotExistsAsync(movies_table_name);

           
            
            var result = await cosmosDatabase.CreateContainerIfNotExistsAsync(new ContainerProperties() { PartitionKeyPath = "/" + partitionKey, Id = new_collection_name }, provisionedThroughput);
            moviesContainer = result.Container;
            if (result.StatusCode == System.Net.HttpStatusCode.Created ||
                result.StatusCode == System.Net.HttpStatusCode.OK)
                operationFailed = false;
            else
                operationFailed = true;
        }
    }
}
