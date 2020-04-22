using System;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DocumentModel;
using System.Collections.Generic;

namespace CosmosDB_viz_DynamoDB
{
  public static partial class CosmosDBvDynamoDB
  {
    /*--------------------------------------------------------------------------
     *                             SearchListing_async
     *--------------------------------------------------------------------------*/
    public static async Task<bool> SearchListing_async( Search search )
    {
      int i = 0;
      List<Document> docList = new List<Document>( );

      Console.WriteLine( "         Here are the movies retrieved:\n" +
                         "         --------------------------------------------------------------------------" );
      Task<List<Document>> getNextBatch;
      operationSucceeded = false;
      operationFailed = false;

      do
      {
        try
        {
          getNextBatch = search.GetNextSetAsync( );
          docList = await getNextBatch;
        }
        catch( Exception ex )
        {
          Console.WriteLine( "        FAILED to get the next batch of movies from Search! Reason:\n          " +
                             ex.Message );
          operationFailed = true;
          return ( false );
        }

        foreach( Document doc in docList )
        {
          i++;
          showMovieDocShort( doc );
        }
      } while( !search.IsDone );
      Console.WriteLine( "     -- Retrieved {0} movies.", i );
      operationSucceeded = true;
      return ( true );
    }


    /*--------------------------------------------------------------------------
     *                             ClientQuerying_async
     *--------------------------------------------------------------------------*/
    public static async Task<bool> ClientQuerying_async( QueryRequest qRequest )
    {
      operationSucceeded = false;
      operationFailed = false;

      QueryResponse qResponse;
      try
      {
        Task<QueryResponse> clientQueryTask = client.QueryAsync( qRequest );
        qResponse = await clientQueryTask;
      }
      catch( Exception ex )
      {
        Console.WriteLine( "      The low-level query FAILED, because:\n       {0}.", ex.Message );
        operationFailed = true;
        return ( false );
      }
      Console.WriteLine( "     -- The low-level query succeeded, and returned {0} movies!", qResponse.Items.Count );
      if( !Pause( ) )
      {
        operationFailed = true;
        return ( false );
      }
      Console.WriteLine( "         Here are the movies retrieved:" +
                         "         --------------------------------------------------------------------------" );
      foreach( Dictionary<string, AttributeValue> item in qResponse.Items )
        showMovieAttrsShort( item );

      Console.WriteLine( "     -- Retrieved {0} movies.", qResponse.Items.Count );
      operationSucceeded = true;
      return ( true );
    }
  }
}