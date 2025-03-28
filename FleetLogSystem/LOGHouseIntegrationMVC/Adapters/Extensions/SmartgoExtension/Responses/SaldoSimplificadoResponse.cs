using LOGHouseSystem.Adapters.Extensions.SmartgoExtension.Dto;
using Newtonsoft.Json;

namespace LOGHouseSystem.Adapters.Extensions.SmartgoExtension.Responses
{
    public class SaldoSimplificadoResponse
    {
        [JsonProperty("model")]
        public Model Model { get; set; }

        [JsonProperty("code")]
        public int Code;

        [JsonProperty("errors")]
        public List<object> Errors;

        [JsonProperty("success")]
        public bool Success;
    }

    public class Model
    {
        [JsonProperty("items")]
        public EstoqueSimplificado[] Items { get; set; }

        [JsonProperty("totalItems")]
        public int TotalItems;

        [JsonProperty("pageSize")]
        public int PageSize;

        [JsonProperty("totalPages")]
        public int TotalPages;

        [JsonProperty("page")]
        public int Page;
    }
}
