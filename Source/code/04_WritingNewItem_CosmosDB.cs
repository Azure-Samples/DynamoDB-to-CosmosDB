using System;
using System.Threading.Tasks;

namespace CosmosDB_viz_DynamoDB
{
  public static partial class CosmosDBvDynamoDB
  {
    /*--------------------------------------------------------------------------
     *     WritingNewMovie
     *--------------------------------------------------------------------------*/
    public static async Task WritingNewMovie_async_cosmosDB(MovieModel newitem)
    {
      operationSucceeded = false;
      operationFailed = false;


      /*int year = (int) newItem["year"];
      string name = newItem["title"];*/

      if( ReadingMovie_async_CosmosDB(newitem.Year,newitem.Title, false ) )
        Console.WriteLine( "  The {0} movie \"{1}\" is already in the Movies table...\n" +
                           "  -- No need to add it again... its info is as follows:\n{2}",
                           newitem.Year, newitem.Title, movie_record_cosmosdb.PrintInfo() );
      else
      {
        try
        {
                   var writeNew= moviesContainer.CreateItemAsync(newitem,new Microsoft.Azure.Cosmos.PartitionKey(newitem.Year));
//          Task<Document> writeNew = moviesTable.PutItemAsync(newItem, token);
          Console.WriteLine("  -- Writing a new movie to the Movies table...");
          await writeNew;
          Console.WriteLine("      -- Wrote the item successfully!");
          operationSucceeded = true;
        }
        catch (Exception ex)
        {
          Console.WriteLine("      FAILED to write the new movie, because:\n       {0}.", ex.Message);
          operationFailed = true;
        }
      }
    }
  }
}