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
        private DynamoDBContext _context;
        private AmazonDynamoDBClient _client;

        public DynamoDbPersister()
        {
            var envConfig = Singleton.Get<EnvironmentConfig>();
            var accessKey = envConfig["aws"]?["access_key"]?.ToString();
            var secretKey = envConfig["aws"]?["secret_key"]?.ToString();
            var endpoint = envConfig["aws"]["endpoint"].ToString();
            var credentials = new BasicAWSCredentials(accessKey, secretKey);
            _context = new DynamoDBContext(
                _client = new AmazonDynamoDBClient(credentials, new AmazonDynamoDBConfig
                {
                    RegionEndpoint = RegionEndpoint.USEast2,
                    ServiceURL = endpoint
                }));
        }

        public virtual async Task<List<T>> Scan<T>(string tableName, ScanFilter scanFilter = null) where T : IObject
        {
            Table table = Table.LoadTable(_client, tableName);
            Search search = table.Scan(scanFilter ?? new ScanFilter());
            List<T> results = new List<T>();
            do
            {
                List<Document> documents = await search.GetNextSetAsync();
                foreach (var document in documents) // for each row
                {
                    T t = (T)typeof(T).GetMethod("FromDocument", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, new[] { document });
                    if (t != default)
                        results.Add(t);
                }
            } while (!search.IsDone);

            return results;
        }

        /// <summary>
        /// Asynchronously retrieves the object of type <typeparamref name="T"/> with the given <paramref name="guid"/> and invokes the <paramref name="callback"/>.
        /// </summary>
        /// <typeparam name="T">The type of IObject.</typeparam>
        /// <param name="guid">The guid of the IObject.</param>
        /// <param name="callback">The optional callback action that gets invoked with the result after retrieval.</param>
        public virtual async Task<T> Get<T>(Guid guid) where T : IObject
        {
            var result = await _context.LoadAsync<T>(guid);
            return result;
        }

        /// <summary>
        /// Asynchronously saves the specified persistable object <paramref name="obj"/>.
        /// </summary>
        /// <typeparam name="T">The type of IObject.</typeparam>
        /// <param name="obj">The persistable object.</param>
        public virtual async void Save<T>(T obj, Action callback = null) where T : IObject
        {
            await _context.SaveAsync(obj);
        }
    }
}
