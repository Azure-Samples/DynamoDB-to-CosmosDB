using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;

namespace CosmosDB_viz_DynamoDB
{
  public static partial class CosmosDBvDynamoDB
  {
    /*--------------------------------------------------------------------------
     *     WritingNewMovie
     *--------------------------------------------------------------------------*/
    public static async Task WritingNewMovie_async( Document newItem )
    {
      operationSucceeded = false;
      operationFailed = false;

      int year = (int) newItem["year"];
      string name = newItem["title"];

      if( await ReadingMovie_async( year, name, false ) )
        Console.WriteLine( "  The {0} movie \"{1}\" is already in the Movies table...\n" +
                           "  -- No need to add it again... its info is as follows:\n{2}",
                           year, name, movie_record.ToJsonPretty( ) );
      else
      {
        try
        {
          Task<Document> writeNew = moviesTable.PutItemAsync(newItem, token);
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