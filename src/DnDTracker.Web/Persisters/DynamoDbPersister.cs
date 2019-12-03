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
using Newtonsoft.Json;

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
        /// <param name="expression">The optional expression (look into AWS Expression documentation).</param>
        public virtual List<T> Scan<T>(Expression expression = null) where T : IObject
        {
            try
            {
                return Task.Run(async () => await ScanAsync<T>(expression)).Result;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to Scan through DynamoDbPersister. " +
                    (Singleton.Get<EnvironmentConfig>().Current == Environments.Local ?
                        "\n\nDid you run ./start-dynamodb.sh?\n\n" : ""), ex);
                return new List<T>();
            }
        }

        /// <summary>
        /// Asynchronously retrieves all the objects of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of IObject.</typeparam>
        /// <param name="expression">The optional expression (look into AWS Expression documentation).</param>
        public virtual async Task<List<T>> ScanAsync<T>(Expression expression = null) where T : IObject
        {
            try
            {
                var tableMap = Singleton.Get<TableMap>();
                var tableName = tableMap[typeof(T)];
                Table table = Table.LoadTable(Client, tableName);
                Search search;
                if (expression != null)
                    search = table.Scan(expression);
                else
                    search = table.Scan(new ScanFilter());
                List<T> results = new List<T>();
                do
                {
                    List<Document> documents = await search.GetNextSetAsync();
                    foreach (var document in documents) // for each row
                    {
                        T t = (T)Activator.CreateInstance(typeof(T));
                        t.FromDocument(document);
                        if (t != null)
                            results.Add(t);
                    }
                } while (!search.IsDone);

                return results;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to ScanAsync through DynamoDbPersister. {ex}" +
                    (Singleton.Get<EnvironmentConfig>().Current == Environments.Local ?
                        "\n\nDid you run ./start-dynamodb.sh?\n\n" : ""), ex);
                return new List<T>();
            }
        }

        /// <summary>
        /// Asynchronously deletes the object of type <typeparamref name="T"/> with the specified <paramref name="guid"/>.
        /// </summary>
        /// <typeparam name="T">The type of IObject.</typeparam>
        /// <param name="guid">The <see cref="Guid"/> of the persistable object.</param>
        public virtual async void Delete<T>(Guid guid) where T : IObject
        {
            try
            {
                var tableMap = Singleton.Get<TableMap>();
                var tableName = tableMap[typeof(T)];
                if (string.IsNullOrEmpty(tableName))
                {
                    Log.Error($"Tried to save an IObject {typeof(T).Name} without an entry in TableMap.");
                    return;
                }
                Table table = Table.LoadTable(Client, tableName);
                await table.DeleteItemAsync(new Primitive(guid.ToString()));
            }
            catch (Exception ex)
            {
                Log.Error("Failed to Delete through DynamoDbPersister.", ex);
            }
        }

        /// <summary>
        /// Synchronously retrieves the object of type <typeparamref name="T"/> with the given <paramref name="guid"/> and invokes the <paramref name="callback"/>.
        /// </summary>
        /// <typeparam name="T">The type of IObject.</typeparam>
        /// <param name="guid">The guid of the IObject.</param>
        public virtual T Get<T>(Guid guid) where T : IObject
        {
            try
            {
                return Task.Run(async () => await GetAsync<T>(guid)).Result;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to Get through DynamoDbPersister.", ex);
                return default;
            }
        }

        /// <summary>
        /// Asynchronously retrieves the object of type <typeparamref name="T"/> with the given <paramref name="guid"/> and invokes the <paramref name="callback"/>.
        /// </summary>
        /// <typeparam name="T">The type of IObject.</typeparam>
        /// <param name="guid">The guid of the IObject.</param>
        public virtual async Task<T> GetAsync<T>(Guid guid) where T : IObject
        {
            try
            {
                var tableMap = Singleton.Get<TableMap>();
                var tableName = tableMap[typeof(T)];
                if (string.IsNullOrEmpty(tableName))
                {
                    Log.Error($"Tried to retrieve an IObject {typeof(T).Name} without an entry in TableMap.");
                    return default;
                }
                var result = await Context.LoadAsync<T>(guid, new DynamoDBOperationConfig()
                {
                    OverrideTableName = tableName
                });
                return result;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to GetAsync through DynamoDbPersister.", ex);
                return default;
            }
        }

        /// <summary>
        /// Asynchronously updates the specified persistable object <paramref name="obj"/>.
        /// </summary>
        /// <typeparam name="T">The type of IObject.</typeparam>
        /// <param name="obj">The persistable object.</param>
        public virtual async void Update<T>(T obj) where T : IObject
        {
            try
            {
                var tableMap = Singleton.Get<TableMap>();
                var tableName = tableMap[typeof(T)];
                if (string.IsNullOrEmpty(tableName))
                {
                    Log.Error($"Tried to save an IObject {typeof(T).Name} without an entry in TableMap.");
                    return;
                }
                Table table = Table.LoadTable(Client, tableName);
                Document document = Context.ToDocument(obj);
                await table.UpdateItemAsync(document, obj.Guid.ToString());
            }
            catch (Exception ex)
            {
                Log.Error("Failed to Update through DynamoDbPersister.", ex);
            }
        }

        /// <summary>
        /// Asynchronously saves the specified persistable object <paramref name="obj"/>.
        /// </summary>
        /// <typeparam name="T">The type of IObject.</typeparam>
        /// <param name="obj">The persistable object.</param>
        public virtual async void Save<T>(T obj) where T : IObject
        {
            try
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
            catch (Exception ex)
            {
                Log.Error("Failed to Save through DynamoDbPersister.", ex);
            }
        }
    }
}
