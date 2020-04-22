using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CosmosDB_viz_DynamoDB
{
  public static partial class CosmosDBvDynamoDB
  {

    /*--------------------------------------------------------------------------
     *     LoadingData_async
     *--------------------------------------------------------------------------*/
    public static async Task LoadingData_async( Table table, string filePath )
    {
      JArray movieArray;

      movieArray = await ReadJsonMovieFile_async( filePath );
      if( movieArray != null )
        await LoadJsonMovieData_async( table, movieArray );
    }

    /*--------------------------------------------------------------------------
     *                             ReadJsonMovieFile_async
     *--------------------------------------------------------------------------*/
    public static async Task<JArray> ReadJsonMovieFile_async( string JsonMovieFilePath )
    {
      StreamReader sr = null;
      JsonTextReader jtr = null;
      JArray movieArray = null;

      Console.WriteLine( "  -- Reading the movies data from a JSON file..." );
      operationSucceeded = false;
      operationFailed = false;
      try
      {
        sr = new StreamReader( JsonMovieFilePath );
        jtr = new JsonTextReader( sr );
        movieArray = (JArray) await JToken.ReadFromAsync( jtr );
        operationSucceeded = true;
      }
      catch( Exception ex )
      {
        Console.WriteLine( "     ERROR: could not read the file!\n          Reason: {0}.", ex.Message );
        operationFailed = true;
      }
      finally
      {
        if( jtr != null )
          jtr.Close( );
        if( sr != null )
          sr.Close( );
      }
      if( operationSucceeded )
      {
        Console.WriteLine( "     -- Succeeded in reading the JSON file!" );
        return ( movieArray );
      }
      return ( null );
    }


    /*--------------------------------------------------------------------------
     *                LoadJsonMovieData_async
     *--------------------------------------------------------------------------*/
    public static async Task LoadJsonMovieData_async( Table moviesTable, JArray moviesArray )
    {
      operationSucceeded = false;
      operationFailed = false;

      int n = moviesArray.Count;
      Console.Write( "     -- Starting to load {0:#,##0} movie records into the Movies table asynchronously...\n" + "" +
        "        Wrote: ", n );
      for( int i = 0, j = 99; i < n; i++ )
      {
        try
        {
          string itemJson = moviesArray[i].ToString();
          Document doc = Document.FromJson(itemJson);
          Task putItem = moviesTable.PutItemAsync(doc);
          if( i >= j )
          {
            j++;
            Console.Write( "{0,5:#,##0}, ", j );
            if( j % 1000 == 0 )
              Console.Write( "\n               " );
            j += 99;
          }
          await putItem;
        }
        catch( Exception ex )
        {
          Console.WriteLine( "\n     ERROR: Could not write the movie record #{0:#,##0}, because:\n       {1}",
                             i, ex.Message );
          operationFailed = true;
          break;
        }
      }
      if( !operationFailed )
      {
        operationSucceeded = true;
        Console.WriteLine( "\n     -- Finished writing all movie records!" );
      }
    }
  }
}