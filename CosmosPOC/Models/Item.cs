namespace CosmosPOC.Models
{
    using Newtonsoft.Json;

    public class Item
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        public string PartitionKey => Id;

        public Item()
        {
        }
        public Item(string name)
        {
            this.Name = name;
        }
    }
}
