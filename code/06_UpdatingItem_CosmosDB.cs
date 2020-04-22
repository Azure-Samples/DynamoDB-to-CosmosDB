using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Cosmos;

namespace CosmosDB_viz_DynamoDB
{
  public static partial class CosmosDBvDynamoDB
  {
    /*--------------------------------------------------------------------------
     *                             UpdatingMovie_async
     *--------------------------------------------------------------------------*/
    public async static Task<bool> UpdatingMovie_async_CosmosDB( MovieModel updatedMovieModel, bool report )
    {
     // UpdateItemResponse updateResponse = null;

      operationSucceeded = false;
      operationFailed = false;
      if( report )
      {
        Console.WriteLine( "  -- Trying to update a movie item..." );
       // updateRequest.ReturnValues = "ALL_NEW";
      }

      try
      {
              
        await moviesContainer.UpsertItemAsync<MovieModel>(updatedMovieModel);
        Console.WriteLine( "     -- SUCCEEDED in updating the movie item!" );
      }
      catch( Exception ex )
      {
        Console.WriteLine( "     -- FAILED to update the movie item, because:\n       {0}.", ex.Message );
        //if( updateResponse != null )
          //Console.WriteLine( "     -- The status code was " + updateResponse.HttpStatusCode.ToString( ) );
        operationFailed = true;return ( false );
      }
      if( report )
      {
        Console.WriteLine( "     Here is the updated movie informtion:" );
       // Console.WriteLine( movieAttributesToJson( updateResponse.Attributes ) );
      }
      operationSucceeded = true;
      return ( true );
    }
  }
}

