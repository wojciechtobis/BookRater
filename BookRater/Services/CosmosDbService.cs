namespace BookRater.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using BookRater.Models;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Azure.Cosmos.Fluent;
    using Microsoft.Extensions.Configuration;

    public class CosmosDbService : ICosmosDbService
    {
        private Container _container;

        public CosmosDbService(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            this._container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task AddItemAsync(Book book)
        {
            await this._container.CreateItemAsync<Book>(book, new PartitionKey(book.Id));
        }

        public async Task DeleteItemAsync(string id)
        {
            await this._container.DeleteItemAsync<Book>(id, new PartitionKey(id));
        }

        public async Task<Book> GetItemAsync(string id)
        {
            try
            {
                ItemResponse<Book> response = await this._container.ReadItemAsync<Book>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

        }

        public async Task<IEnumerable<Book>> GetItemsAsync(string queryString)
        {
            var query = this._container.GetItemQueryIterator<Book>(new QueryDefinition(queryString));
            List<Book> results = new List<Book>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task UpdateItemAsync(string id, Book book)
        {
            await this._container.UpsertItemAsync<Book>(book, new PartitionKey(id));
        }
    }
}
