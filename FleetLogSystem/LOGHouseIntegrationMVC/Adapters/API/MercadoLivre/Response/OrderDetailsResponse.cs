using Newtonsoft.Json;
using System.Collections.Generic;

namespace LOGHouseSystem.Adapters.API.MercadoLivre.Response
{
    public partial class OrderDetailsResponse
    {
        [JsonProperty("query")]
        public string Query { get; set; }

        [JsonProperty("results")]
        public List<Result> Results { get; set; }

    }

    public partial class Result
    {

        [JsonProperty("shipping")]
        public Shipping Shipping { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("pack_id")]
        public object PackId { get; set; }

    }

    public partial class Shipping
    {
        [JsonProperty("id")]
        public long Id { get; set; }
    }
}
