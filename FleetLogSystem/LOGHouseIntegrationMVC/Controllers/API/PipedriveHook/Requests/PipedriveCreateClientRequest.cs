using System.Text.Json.Serialization;

namespace LOGHouseSystem.Controllers.API.PipedriveHook.Requests
{
    public class PipedriveCreateClientRequest
    {
        [JsonPropertyName("armazenagem")]
        public int Armazenagem { get; set; }

        [JsonPropertyName("armazenagem-excedente")]
        public int ArmazenagemExcedente { get; set; }

        [JsonPropertyName("armazenagem-valor")]
        public decimal ArmazenagemValor { get; set; }

        [JsonPropertyName("cnpj")]
        public string Cnpj { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("endereco")]
        public string Endereco { get; set; }

        [JsonPropertyName("pedidos")]
        public int Pedidos { get; set; }

        [JsonPropertyName("pedidos-valor")]
        public decimal PedidosValor { get; set; }

        [JsonPropertyName("razao-social")]
        public string RazaoSocial { get; set; }

        [JsonPropertyName("telefone")]
        public string Telefone { get; set; }

        [JsonPropertyName("unidades-envio")]
        public int UnidadesEnvio { get; set; }

        [JsonPropertyName("valor-contrato")]
        public decimal ValorContrato { get; set; }
    }
}
