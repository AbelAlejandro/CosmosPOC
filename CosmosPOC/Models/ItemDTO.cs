using System.Collections.Generic;

namespace CosmosPOC.Models
{
    public class ItemDTO
    {
        public int Size { get; set; }
        public int SizeEncoded { get; set; }
        public string ContinuationToken { get; set; }
        public IEnumerable<Item> Items { get; set; }
        
    }
}
