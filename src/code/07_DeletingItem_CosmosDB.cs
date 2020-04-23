using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.Azure.Cosmos;

namespace CosmosDB_viz_DynamoDB
{
  public static partial class CosmosDBvDynamoDB
  {

    /*--------------------------------------------------------------------------
     *                       DeletingItem_async
     *--------------------------------------------------------------------------*/
    public static async Task<bool> DeletingItem_async_CosmosDB( string query )
    {
    
      operationSucceeded = false;
      operationFailed = false;

            // Create Primitives for the HASH and RANGE portions of the primary key
            // Primitive hash = new Primitive(year.ToString(), true);
            //Primitive range = new Primitive(title, false);
            //  DeleteItemOperationConfig deleteConfig = new DeleteItemOperationConfig( );
            //deleteConfig.ConditionalExpression = condition;
            //deleteConfig.ReturnValues = ReturnValues.AllOldAttributes;

            
//      Console.WriteLine( "  -- Trying to delete the {0} movie \"{1}\"...", year, title );
      try
      {
       var result= ReadingMovieItem_async_List_CosmosDB(query);
                while (result.HasMoreResults)
                {
                    var resultModel = await result.ReadNextAsync();
                    foreach (var movie in resultModel.ToList<MovieModel>())
                    {

                        var delItem = moviesContainer.DeleteItemAsync<MovieModel>(movie.Id, new PartitionKey(movie.Year));
                        var deletedItem = await delItem;
                        Console.WriteLine("     -- SUCCEEDED in deleting the movie record that looks like this:\n" +
                                                      deletedItem.Resource.ToString());
                    }
                }
            }
      catch( Exception ex )
      {
        Console.WriteLine( "     FAILED to delete the movie item, for this reason:\n       {0}\n", ex.Message );
        operationFailed = true;
        return ( false );
      }
      operationSucceeded = true;
      return ( true );
    }
  }
}