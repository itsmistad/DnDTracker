using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using DnDTracker.Web;
using DnDTracker.Web.Objects;
using DnDTracker.Web.Persisters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using DnDTracker.Web.Configuration;

namespace DnDTracker.Migrations.Helpers
{
    public class DynamoDbHelper
    {
        private static DynamoDbPersister Persister => Singleton.Get<DynamoDbPersister>();
        public static bool EnableForceSave { get; set; }

        private static void WaitUntilActive(string tableName)
        {
            var envConfig = Singleton.Get<EnvironmentConfig>();
            if (envConfig.Current == Environments.Local)
                return;

            Console.WriteLine($"   - Waiting for {tableName} to be ACTIVE");
            bool success;
            Task<DescribeTableResponse> task = null;
            do
            {
                if (task != null)
                    Thread.Sleep(3000);
                task = Task.Run(async () => await Persister.Client.DescribeTableAsync(tableName));
                success = task.Result.Table.TableStatus == TableStatus.ACTIVE;
            } while (!success);
        }

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
                Console.WriteLine($"   - CreateTableRequest for {request.TableName} returned {response.HttpStatusCode}");
                WaitUntilActive(request.TableName);
            }
        }

        public static void UpdateTable<T>(UpdateTableRequest updateTableRequest) where T : IObject
        {
            if (TableExists<T>())
            {
                var tableMap = Singleton.Get<TableMap>();
                updateTableRequest.TableName = tableMap[typeof(T)];
                var response = Task.Run(async () => await Persister.Client.UpdateTableAsync(updateTableRequest)).Result;
                Console.WriteLine($"   - UpdateTableRequest for {updateTableRequest.TableName} returned {response.HttpStatusCode}");
                WaitUntilActive(updateTableRequest.TableName);
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
                Console.WriteLine($"   - DeleteTableRequest for {request.TableName} returned {response.HttpStatusCode}");
            }
        }

        public static void Delete<T>(Guid guid) where T : IObject
        {
            if (TableExists<T>())
                Persister.Delete<T>(guid);
        }

        public static void Save<T>(T obj) where T : IObject
        {
            if (TableExists<T>() && (Get<T>(obj.Guid) == null || EnableForceSave))
                Persister.Save(obj);
        }

        public static List<T> Scan<T>(Expression expression = null) where T : IObject
        {
            return TableExists<T>() ? Persister.Scan<T>(expression) : new List<T>();
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
                Console.WriteLine(ex.ToString());
            }

            return false;
        }
    }
}
