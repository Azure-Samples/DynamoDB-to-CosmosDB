using System;
using System.Net.Sockets;
using Amazon.DynamoDBv2;
using Microsoft.Azure.Cosmos;
namespace CosmosDB_viz_DynamoDB
{
  public static partial class CosmosDBvDynamoDB
  {
    /*-----------------------------------------------------------------------------------
      *  If you are creating a client for the DynamoDB service, make sure your credentials
      *  are set up first, as explained in:
      *  https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/SettingUp.DynamoWebService.html,
      *
      *  If you are creating a client for DynamoDBLocal (for testing purposes),
      *  DynamoDB-Local should be started first. For most simple testing, you can keep
      *  data in memory only, without writing anything to disk.  To do this, use the
      *  following command line:
      *
      *    java -Djava.library.path=./DynamoDBLocal_lib -jar DynamoDBLocal.jar -inMemory
      *
      *  For information about DynamoDBLocal, see:
      *  https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/DynamoDBLocal.html.
      *-----------------------------------------------------------------------------------*/
    /*--------------------------------------------------------------------------
     *          CreateClient
     *--------------------------------------------------------------------------*/
    public static bool CreateClient_CosmosDB( bool useLocal )
    {
      if( useLocal )
      {
        operationSucceeded = false;
        operationFailed = false;

        // First, check to see whether anyone is listening on the DynamoDB local port
        // (by default, this is port 8000, so if you are using a different port, modify this accordingly)
        bool localFound;
        try
        {
                    client_cosmosDB = new CosmosClient("AccountEndpoint=https://postbuilddemo-cf.documents.azure.com:443/;AccountKey=T6oQdPBqOJKJRFvDaHyGGybLlHkPMtSlms0ko03rQwDxdLxUIEsekZxAOxKxRfOkwBxWfqU4EehJy2Mt3Ndj3A==;");
                    //using (var tcp_client = new Microsoft.Azure.Cosmos.DocumentClient(new Uri("http://localhost:8081"), "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="))
                    //{
                    //client_cosmosDB = tcp_client;
                    //}
                    localFound = true;
        }
        catch
        {
          localFound =  false;
        }
        if( !localFound )
        {
          Console.WriteLine("\n      ERROR: DynamoDB Local does not appear to have been started..." +
                            "\n        (checked port 8000)");
          operationFailed = true;
          return (false);
        }

      // If DynamoDB-Local does seem to be running, so create a client
        Console.WriteLine( "  -- Setting up a DynamoDB-Local client (DynamoDB Local seems to be running)" );
                AmazonDynamoDBConfig ddbConfig = new AmazonDynamoDBConfig
                {
                    ServiceURL = "http://localhost:8000"
                };
                try { client = new AmazonDynamoDBClient( ddbConfig ); }
        catch( Exception ex )
        {
          Console.WriteLine( "     FAILED to create a DynamoDBLocal client; " + ex.Message );
          operationFailed = true;
          return false;
        }
      }

      else
      {
        try { client = new AmazonDynamoDBClient( ); }
        catch( Exception ex )
        {
          Console.WriteLine( "     FAILED to create a DynamoDB client; " + ex.Message );
          operationFailed = true;
        }
      }
      operationSucceeded = true;
      return true;
    }
  }
}
