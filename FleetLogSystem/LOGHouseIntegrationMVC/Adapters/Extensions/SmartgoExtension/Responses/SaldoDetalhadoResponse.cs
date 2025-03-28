using Newtonsoft.Json;

namespace LOGHouseSystem.Adapters.Extensions.SmartgoExtension.Responses
{
    public class SaldoDetalhadoResponse
    {
        [JsonProperty("model")]
        public List<SaldoDetalhado> Model { get; set; }

        [JsonProperty("code")]
        public int Code;

        [JsonProperty("errors")]
        public List<object> Errors;

        [JsonProperty("success")]
        public bool Success;
    }

    public class SaldoDetalhado
    {
        [JsonProperty("protocoloDeDeposito")]
        public string ProtocoloDeDeposito;

        [JsonProperty("iD_Deposito")]
        public int IDDeposito;

        [JsonProperty("estruturaCodigo")]
        public string EstruturaCodigo;

        [JsonProperty("iD_Estrutura")]
        public int IDEstrutura;

        [JsonProperty("iD_Produto")]
        public int IDProduto;

        [JsonProperty("produtoNome")]
        public string ProdutoNome;

        [JsonProperty("produtoNCM")]
        public int ProdutoNCM;

        [JsonProperty("produtoCodigoInterno")]
        public string ProdutoCodigoInterno;

        [JsonProperty("produtoCodigoExterno")]
        public string ProdutoCodigoExterno;

        [JsonProperty("dataDeEntrada")]
        public DateTime DataDeEntrada;

        [JsonProperty("quantidade")]
        public int Quantidade;

        [JsonProperty("quantidadeProdutosEmbalagem")]
        public int QuantidadeProdutosEmbalagem;

        [JsonProperty("valorUnitario")]
        public string ValorUnitario;

        [JsonProperty("iD_Depositante")]
        public int IDDepositante;

        [JsonProperty("depositante")]
        public string Depositante;

        [JsonProperty("quantidadeDeMovimentacao")]
        public int QuantidadeDeMovimentacao;

        [JsonProperty("tipoUnidadeMovimentacao")]
        public string TipoUnidadeMovimentacao;

        [JsonProperty("tipoUnidadeEmbalagem")]
        public string TipoUnidadeEmbalagem;

        [JsonProperty("unidadeProduto")]
        public string UnidadeProduto;

        [JsonProperty("versao")]
        public int Versao;

        [JsonProperty("enderecoLogico")]
        public string EnderecoLogico;

        [JsonProperty("iD_PedidoRecebimento")]
        public int IDPedidoRecebimento;

        [JsonProperty("notaFiscal")]
        public string NotaFiscal;

        [JsonProperty("diasEmEstoque")]
        public int DiasEmEstoque;

        [JsonProperty("quantidadeEstoque")]
        public int QuantidadeEstoque;

        [JsonProperty("embalagemPrimaria")]
        public string EmbalagemPrimaria;

        [JsonProperty("fracionada")]
        public string Fracionada;

        [JsonProperty("quantidadeDisponivel")]
        public int QuantidadeDisponivel;

        [JsonProperty("observacaoPedido")]
        public string ObservacaoPedido;

        [JsonProperty("totalRows")]
        public int TotalRows;

        [JsonProperty("recebimentoCodigoInterno")]
        public string RecebimentoCodigoInterno;

        [JsonProperty("recebimentoCodigoExterno")]
        public string RecebimentoCodigoExterno;

        [JsonProperty("descricaoProduto")]
        public string DescricaoProduto;

        [JsonProperty("lote")]
        public string Lote { get; set; }

        [JsonProperty("dataDeValidade")]
        public string DataDeValidade { get; set; }
    }
}
