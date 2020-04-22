using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.Model;

namespace CosmosDB_viz_DynamoDB
{
  public static partial class CosmosDBvDynamoDB
  {
    /*--------------------------------------------------------------------------
     *                             UpdatingMovie_async
     *--------------------------------------------------------------------------*/
    public static async Task<bool> UpdatingMovie_async( UpdateItemRequest updateRequest, bool report )
    {
      UpdateItemResponse updateResponse = null;

      operationSucceeded = false;
      operationFailed = false;
      if( report )
      {
        Console.WriteLine( "  -- Trying to update a movie item..." );
        updateRequest.ReturnValues = "ALL_NEW";
      }

      try
      {
        updateResponse = await client.UpdateItemAsync( updateRequest );
        Console.WriteLine( "     -- SUCCEEDED in updating the movie item!" );
      }
      catch( Exception ex )
      {
        Console.WriteLine( "     -- FAILED to update the movie item, because:\n       {0}.", ex.Message );
        if( updateResponse != null )
          Console.WriteLine( "     -- The status code was " + updateResponse.HttpStatusCode.ToString( ) );
        operationFailed = true;return ( false );
      }
      if( report )
      {
        Console.WriteLine( "     Here is the updated movie informtion:" );
        Console.WriteLine( movieAttributesToJson( updateResponse.Attributes ) );
      }
      operationSucceeded = true;
      return ( true );
    }
  }
}

