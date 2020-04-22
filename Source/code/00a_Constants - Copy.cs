/*******************************************************************************
* Copyright 2009-2018 Amazon.com, Inc. or its affiliates. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License"). You may
* not use this file except in compliance with the License. A copy of the
* License is located at
*
* http://aws.amazon.com/apache2.0/
*
* or in the "license" file accompanying this file. This file is
* distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
* KIND, either express or implied. See the License for the specific
* language governing permissions and limitations under the License.
*******************************************************************************/
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
using System.Collections.Generic;
using Amazon.DynamoDBv2.Model;

namespace DynamoDB_intro
{
  public partial class Ddb_Intro
  {
    /*==========================================================================
     *      Static Values Used by this introductory sample
     *==========================================================================*/
    /*---------------------------------------------------------
     *    1.  The data used to create a new table
     *---------------------------------------------------------*/
    // movies_table_name
    public const string movies_table_name = "Movies";

    // key names for the Movies table
    public const string partition_key_name = "year";
    public const string sort_key_name      = "title";

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

    // movies_table_provisioned_throughput
    public static ProvisionedThroughput movies_table_provisioned_throughput
      = new ProvisionedThroughput( 1, 1 );


    /*---------------------------------------------------------
     *    2.  The path to the JSON movies data file to load
     *---------------------------------------------------------*/
    public const string movieDataPath = "./moviedata.json";
  }
}
