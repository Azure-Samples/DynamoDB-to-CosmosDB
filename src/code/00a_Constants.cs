using System.Collections.Generic;
using Amazon.DynamoDBv2.Model;

namespace CosmosDB_viz_DynamoDB
{
  public static partial class CosmosDBvDynamoDB
  {
    /*==========================================================================
     *      Constant/Static Values Used by this introductory sample
     *==========================================================================*/
    public const string commaSep = ", ";
    public const string stepString =
      "\n--------------------------------------------------------------------------------------" +
      "\n    STEP {0}:  {1}" +
      "\n--------------------------------------------------------------------------------------";

    /*---------------------------------------------------------
     *    1.  The data used to create a new table
     *---------------------------------------------------------*/
    // movies_table_name
    public const string movies_table_name = "Movies";

        public const string movies_database_path = movies_table_name;

    // key names for the Movies table
    public const string partition_key_name = "year";
    public const string sort_key_name      = "title";
        public const int movie_collection_provisioned_throughput = 400;
       

        // movie_items_attributes
        public static List<AttributeDefinition> movie_items_attributes
      = new List<AttributeDefinition>
    {
      new AttributeDefinition
      {
        AttributeName = partition_key_name,
        AttributeType = "N"
      },
      new AttributeDefinition
      {
        AttributeName = sort_key_name,
        AttributeType = "S"
      }
    };

    // movies_key_schema
    public static List<KeySchemaElement> movies_key_schema
      = new List<KeySchemaElement>
    {
      new KeySchemaElement
      {
        AttributeName = partition_key_name,
        KeyType = "HASH"
      },
      new KeySchemaElement
      {
        AttributeName = sort_key_name,
        KeyType = "RANGE"
      }
    };
        public const int readUnits = 1, writeUnits = 1;
    // movies_table_provisioned_throughput
    public static ProvisionedThroughput movies_table_provisioned_throughput
      = new ProvisionedThroughput( readUnits, writeUnits );


    /*---------------------------------------------------------
     *    2.  The path to the JSON movies data file to load
     *---------------------------------------------------------*/
    public const string movieDataPath = "./moviedata.json";
  }
}
