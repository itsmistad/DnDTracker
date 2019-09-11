using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using DnDTracker.Web;
using DnDTracker.Web.Objects;
using DnDTracker.Web.Persisters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DnDTracker.Migrations.Helpers
{
    public class DynamoDbHelper
    {
        private static DynamoDbPersister Persister => Singleton.Get<DynamoDbPersister>();

        public static void CreateTable<T>() where T : IObject
        {
            if (!TableExists<T>())
            {
                var tableMap = Singleton.Get<TableMap>();
                var request = new CreateTableRequest
                {
                    AttributeDefinitions = new List<AttributeDefinition>()
                    {
                        new AttributeDefinition
                        {
                            AttributeName = "Guid",
                            AttributeType = "S"
                        },
                        new AttributeDefinition
                        {
                            AttributeName = "CreateDate",
                            AttributeType = "S"
                        }
                    },
                    KeySchema = new List<KeySchemaElement>
                    {
                        new KeySchemaElement
                        {
                            AttributeName = "Guid",
                            KeyType = "HASH" //Partition key
                        },
                        new KeySchemaElement
                        {
                            AttributeName = "CreateDate",
                            KeyType = "RANGE" //Sort key
                        }
                    },
                    ProvisionedThroughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 5,
                        WriteCapacityUnits = 5
                    },
                    TableName = tableMap[typeof(T)]
                };

                Task.Run(async () => await Persister.Client.CreateTableAsync(request));
            }
        }

        public static void UpdateTable<T>(UpdateTableRequest request) where T : IObject
        {
            if (TableExists<T>())
            {
                var tableMap = Singleton.Get<TableMap>();
                request.TableName = tableMap[typeof(T)];
                Task.Run(async () => await Persister.Client.UpdateTableAsync(request));
            }
        }

        public static void DeleteTable<T>() where T : IObject
        {
            if (TableExists<T>())
            {
                var tableMap = Singleton.Get<TableMap>();
                Persister.Client.DeleteTableAsync(new DeleteTableRequest()
                {
                    TableName = tableMap[typeof(T)]
                });
            }
        }

        public static void Save<T>(T obj) where T : IObject
        {
            if (TableExists<T>())
                Persister.Save(obj);
        }

        public static List<T> Scan<T>(ScanFilter scanFilter = null) where T : IObject
        {
            return TableExists<T>() ? Persister.Scan<T>(scanFilter) : new List<T>();
        }

        public static T Get<T>(Guid guid) where T : IObject
        {
            return TableExists<T>() ? Persister.Get<T>(guid) : default;
        }

        private static bool TableExists<T>() where T : IObject
        {
            var tableMap = Singleton.Get<TableMap>();
            try
            {
                return Task.Run(async () => await Persister.Client.DescribeTableAsync(new DescribeTableRequest()
                {
                    TableName = tableMap[typeof(T)]
                })).Result.Table.TableStatus == "ACTIVE";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return false;
        }
    }
}
