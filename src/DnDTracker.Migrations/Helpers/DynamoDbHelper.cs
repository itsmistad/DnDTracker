using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using DnDTracker.Web;
using DnDTracker.Web.Objects;
using DnDTracker.Web.Persisters;
using System;
using System.Collections.Generic;
using System.Net;
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
                        }
                    },
                    KeySchema = new List<KeySchemaElement>
                    {
                        new KeySchemaElement
                        {
                            AttributeName = "Guid",
                            KeyType = "HASH" //Partition key
                        }
                    },
                    ProvisionedThroughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 5,
                        WriteCapacityUnits = 5
                    },
                    TableName = tableMap[typeof(T)]
                };
                var response = Task.Run(async () => await Persister.Client.CreateTableAsync(request)).Result;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($"   - CreateTableRequest for {request.TableName} returned {response.HttpStatusCode}");
            }
        }

        public static void UpdateTable<T>(UpdateTableRequest updateTableRequest) where T : IObject
        {
            if (TableExists<T>())
            {
                var tableMap = Singleton.Get<TableMap>();
                updateTableRequest.TableName = tableMap[typeof(T)];
                var response = Task.Run(async () => await Persister.Client.UpdateTableAsync(updateTableRequest)).Result;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($"   - UpdateTableRequest for {updateTableRequest.TableName} returned {response.HttpStatusCode}");
            }
        }

        public static void DeleteTable<T>() where T : IObject
        {
            if (TableExists<T>())
            {
                var tableMap = Singleton.Get<TableMap>();
                var request = new DeleteTableRequest()
                {
                    TableName = tableMap[typeof(T)]
                };
                var response = Task.Run(async () => await Persister.Client.DeleteTableAsync(request)).Result;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($"   - DeleteTableRequest for {request.TableName} returned {response.HttpStatusCode}");
            }
        }

        public static void Save<T>(T obj) where T : IObject
        {
            if (TableExists<T>() && Get<T>(obj.Guid) == default)
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
                return Task.Run(async () => await Persister.Client.ListTablesAsync(new ListTablesRequest()))
                    .Result.TableNames.Contains(tableMap[typeof(T)]);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(ex.ToString());
            }

            return false;
        }
    }
}
