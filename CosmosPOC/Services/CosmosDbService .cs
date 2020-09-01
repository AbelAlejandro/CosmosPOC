using CosmosPOC;
using CosmosPOC.Models;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CosnosPOC
{
    public class CosmosDbService : ICosmosDbService
    {
        private readonly Container _container;

        public CosmosDbService(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            this._container = dbClient.GetContainer(databaseName, containerName);
        }
        public async Task AddItemAsync(Item item)
        {
            await this._container.CreateItemAsync<Item>(item, new PartitionKey(item.Id));
        }

        public async Task<Item> GetItemAsync(string id)
        {
            try
            {
                ItemResponse<Item> response = await this._container.ReadItemAsync<Item>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<(IEnumerable<Item>, string, int)> GetItems(bool change = false, string? continuationToken = null, int itemCount = 50)
        {
            QueryRequestOptions queryOptions = new QueryRequestOptions()
            {
                MaxItemCount = itemCount
            };
            var cont = string.IsNullOrEmpty(continuationToken) ? null : Base64Decode(continuationToken);
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            const string queryString = "select * from c";
            const string altQuery = "SELECT * FROM c WHERE c.name = \"NewItem\"";
            QueryDefinition queryDefinition = null;
            if (change)
            {
                queryDefinition = new QueryDefinition(altQuery);
            }
            else
            {
                queryDefinition = new QueryDefinition(queryString);
            }
            foreach (var queryParam in queryParams.Keys)
            {
                queryDefinition = queryDefinition.WithParameter(queryParam, queryParams[queryParam]);
            }
            var query = _container.GetItemQueryIterator<Item>(queryDefinition, cont, queryOptions);

            List<Item> results = new List<Item>();
            var response = await query.ReadNextAsync();
            results.AddRange(response.ToList<Item>());

            if (!string.IsNullOrEmpty(response.ContinuationToken))
            {
                cont = Base64Encode(response.ContinuationToken);
            }
            if (response.ContinuationToken != null)
                return (results, cont, response.ContinuationToken.Length);
            else
                return (results, "", 0);
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}