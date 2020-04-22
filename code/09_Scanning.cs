using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DocumentModel;

namespace CosmosDB_viz_DynamoDB
{
  public static partial class CosmosDBvDynamoDB
  {
    /*--------------------------------------------------------------------------
     *                             ClientScanning_async
     *--------------------------------------------------------------------------*/
    public static async Task<bool> ClientScanning_async( ScanRequest sRequest )
    {
      operationSucceeded = false;
      operationFailed = false;

      ScanResponse sResponse;
      Task<ScanResponse> clientScan = client.ScanAsync(sRequest);
      try
      {
        sResponse = await clientScan;
      }
      catch( Exception ex )
      {
        Console.WriteLine( "     -- FAILED to retrieve the movies, because:\n        {0}", ex.Message );
        operationFailed = true;
        Pause( );
        return( false );
      }
      Console.WriteLine( "     -- The low-level scan succeeded, and returned {0} movies!", sResponse.Items.Count );
      if( !Pause( ) )
      {
        operationFailed = true;
        return ( false );
      }

      Console.WriteLine( "         Here are the movies retrieved:\n" +
                         "         --------------------------------------------------------------------------" );
      foreach( Dictionary<string, AttributeValue> item in sResponse.Items )
        showMovieAttrsShort( item );

      Console.WriteLine( "     -- Retrieved {0} movies.", sResponse.Items.Count );
      operationSucceeded = true;
      return ( true );
    }
  }
}