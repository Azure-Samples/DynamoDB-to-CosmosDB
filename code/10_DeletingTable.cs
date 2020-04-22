using System;
using System.Threading.Tasks;

namespace CosmosDB_viz_DynamoDB
{
  public static partial class CosmosDBvDynamoDB
  {
    /*--------------------------------------------------------------------------
     *                DeletingTable_async
     *--------------------------------------------------------------------------*/
    public static async Task<bool> DeletingTable_async( string tableName )
    {
      operationSucceeded = false;
      operationFailed = false;

      Console.WriteLine( "  -- Trying to delete the table named \"{0}\"...", tableName );
      Pause( );
      Task tblDelete = client.DeleteTableAsync( tableName );
      try
      {
        await tblDelete;
      }
      catch( Exception ex )
      {
        Console.WriteLine( "     ERROR: Failed to delete the table, because:\n            " + ex.Message );
        operationFailed = true;
        return ( false );
      }
      Console.WriteLine( "     -- Successfully deleted the table!" );
      operationSucceeded = true;
      Pause( );
      return ( true );
    }
  }
}