namespace BookRater.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using BookRater.Models;

    public interface ICosmosDbService
    {
        Task<IEnumerable<Book>> GetItemsAsync(string query);
        Task<Book> GetItemAsync(string id);
        Task AddItemAsync(Book book);
        Task UpdateItemAsync(string id, Book book);
        Task DeleteItemAsync(string id);
    }
}
