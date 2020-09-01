using CosmosPOC.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CosmosPOC
{
    public interface ICosmosDbService
    {
        Task<(IEnumerable<Item>, string, int)> GetItems(bool change = false, string? continuationToken = null, int itemCount = 50);
        Task<Item> GetItemAsync(string id);
        Task AddItemAsync(Item item);
    }
}
