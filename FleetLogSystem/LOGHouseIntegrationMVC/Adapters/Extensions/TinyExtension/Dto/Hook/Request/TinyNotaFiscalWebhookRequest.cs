using System.Text.Json.Serialization;

namespace LOGHouseSystem.Adapters.Extensions.TinyExtension.Dto.Hook.Request
{
    public class TinyInvoiceWebhookRequest
    {
        [JsonPropertyName("cnpj")]
        public string Cnpj { get; set; }

        [JsonPropertyName("idEcommerce")]
        public int IdEcommerce { get; set; }

        [JsonPropertyName("tipo")]
        public string Tipo { get; set; }

        [JsonPropertyName("versao")]
        public string Versao { get; set; }

        [JsonPropertyName("dados")]
        public TinyDataWebhookRequest Dados { get; set; }        

    }

    public class TinyDataWebhookRequest
    {
        [JsonPropertyName("chaveAcesso")]
        public string ChaveAcesso { get; set; }

        [JsonPropertyName("numero")]
        public int Numero { get; set; }

        [JsonPropertyName("serie")]
        public int Serie { get; set; }

        [JsonPropertyName("urlDanfe")]
        public string UrlDanfe { get; set; }

        // Tiny order id
        [JsonPropertyName("idPedidoEcommerce")]
        public string IdPedidoEcommerce { get; set; }

        [JsonPropertyName("dataEmissao")]
        public DateTime DataEmissao { get; set; }

        [JsonPropertyName("valorNota")]
        public float ValorNota { get; set; }

        [JsonPropertyName("idNotaFiscalTiny")]
        public int IdNotaFiscalTiny { get; set; }
    }
}
