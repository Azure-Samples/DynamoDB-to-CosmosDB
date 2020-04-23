using Microsoft.Azure.Cosmos;
using System;
using System.Threading.Tasks;

namespace CosmosDB_viz_DynamoDB
{
  public static partial class CosmosDBvDynamoDB
  {
    /*--------------------------------------------------------------------------
     *                DeletingTable_async
     *--------------------------------------------------------------------------*/
    public static async Task<bool> DeletingCollection_async_CosmosDB( )
    {
      operationSucceeded = false;
      operationFailed = false;

      Console.WriteLine( "  -- Trying to delete the collection named \"{0}\"...", moviesContainer.Id);
      Pause( );
      Task tblDelete = moviesContainer.DeleteContainerAsync();
      try
      {
        await tblDelete;
      }
      catch( Exception ex )
      {
        Console.WriteLine( "     ERROR: Failed to delete the collection, because:\n            " + ex.Message );
        operationFailed = true;
        return ( false );
      }
      Console.WriteLine( "     -- Successfully deleted the Collection!" );
      operationSucceeded = true;
      Pause( );
      return ( true );
    }
        public static async Task<bool> DeletingDB_async_CosmosDB()
        {
            operationSucceeded = false;
            operationFailed = false;

            Console.WriteLine("  -- Trying to delete the table named \"{0}\"...", cosmosDatabase.Id);
            Pause();
            Task tblDelete = cosmosDatabase.DeleteAsync();
            try
            {
                await tblDelete;
            }
            catch (Exception ex)
            {
                Console.WriteLine("     ERROR: Failed to delete the database, because:\n            " + ex.Message);
                operationFailed = true;
                return (false);
            }
            Console.WriteLine("     -- Successfully deleted the databse!");
            operationSucceeded = true;
            Pause();
            return (true);
        }
    }
}