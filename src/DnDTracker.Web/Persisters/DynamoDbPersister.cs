using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using DnDTracker.Web.Configuration;
using DnDTracker.Web.Logging;
using DnDTracker.Web.Objects;

namespace DnDTracker.Web.Persisters
{
    public class DynamoDbPersister
    {
        public DynamoDBContext Context { get; }
        public AmazonDynamoDBClient Client { get; }

        public DynamoDbPersister()
        {
            var envConfig = Singleton.Get<EnvironmentConfig>();
            var accessKey = envConfig["aws"]?["access_key"]?.ToString();
            var secretKey = envConfig["aws"]?["secret_key"]?.ToString();
            var endpoint = envConfig["aws"]?["endpoint"]?.ToString();
            var credentials = new BasicAWSCredentials(accessKey, secretKey);
            var config = new AmazonDynamoDBConfig { RegionEndpoint = RegionEndpoint.USEast2 };
            if (!string.IsNullOrEmpty(endpoint))
                config.ServiceURL = endpoint;
            Context = new DynamoDBContext(Client = new AmazonDynamoDBClient(credentials, config));
        }

        /// <summary>
        /// Synchronously retrieves all the objects of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of IObject.</typeparam>
        /// <param name="scanFilter">The optional scan filter (look into AWS ScanFilter documentation).</param>
        public virtual List<T> Scan<T>(ScanFilter scanFilter = null) where T: IObject
        {
            return Task.Run(async () => await ScanAsync<T>(scanFilter)).Result;
        }

        /// <summary>
        /// Asynchronously retrieves all the objects of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of IObject.</typeparam>
        /// <param name="scanFilter">The optional scan filter (look into AWS ScanFilter documentation).</param>
        public virtual async Task<List<T>> ScanAsync<T>(ScanFilter scanFilter = null) where T : IObject
        {
            var tableMap = Singleton.Get<TableMap>();
            var tableName = tableMap[typeof(T)];
            Table table = Table.LoadTable(Client, tableName);
            Search search = table.Scan(scanFilter ?? new ScanFilter());
            List<T> results = new List<T>();
            do
            {
                List<Document> documents = await search.GetNextSetAsync();
                foreach (var document in documents) // for each row
                {
                    T t = (T)typeof(T).GetMethod("FromDocument", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, new[] { document });
                    if (t != null)
                        results.Add(t);
                }
            } while (!search.IsDone);

            return results;
        }

        /// <summary>
        /// Synchronously retrieves the object of type <typeparamref name="T"/> with the given <paramref name="guid"/> and invokes the <paramref name="callback"/>.
        /// </summary>
        /// <typeparam name="T">The type of IObject.</typeparam>
        /// <param name="guid">The guid of the IObject.</param>
        public virtual T Get<T>(Guid guid) where T : IObject
        {
            return Task.Run(async () => await GetAsync<T>(guid)).Result;
        }

        /// <summary>
        /// Asynchronously retrieves the object of type <typeparamref name="T"/> with the given <paramref name="guid"/> and invokes the <paramref name="callback"/>.
        /// </summary>
        /// <typeparam name="T">The type of IObject.</typeparam>
        /// <param name="guid">The guid of the IObject.</param>
        public virtual async Task<T> GetAsync<T>(Guid guid) where T : IObject
        {
            var tableMap = Singleton.Get<TableMap>();
            var tableName = tableMap[typeof(T)];
            if (string.IsNullOrEmpty(tableName))
            {
                Log.Error($"Tried to retrieve an IObject {typeof(T).Name} without an entry in TableMap.");
                return default;
            }
            try
            {
                var result = await Context.LoadAsync<T>(guid, new DynamoDBOperationConfig()
                {
                    OverrideTableName = tableName
                });
                return result;
            }
            catch (Exception) { }

            return default;
        }

        /// <summary>
        /// Asynchronously saves the specified persistable object <paramref name="obj"/>.
        /// </summary>
        /// <typeparam name="T">The type of IObject.</typeparam>
        /// <param name="obj">The persistable object.</param>
        public virtual async void Save<T>(T obj) where T : IObject
        {
            var tableMap = Singleton.Get<TableMap>();
            var tableName = tableMap[typeof(T)];
            if (string.IsNullOrEmpty(tableName))
            {
                Log.Error($"Tried to save an IObject {typeof(T).Name} without an entry in TableMap.");
                return;
            }
            await Context.SaveAsync(obj, new DynamoDBOperationConfig()
            {
                OverrideTableName = tableName
            });
        }
    }
}
