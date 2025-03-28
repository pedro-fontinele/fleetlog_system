using Newtonsoft.Json;
using System.ComponentModel;

namespace LOGHouseSystem.Adapters.Extensions.SmartgoExtension.Dto
{
    public class EstoqueSimplificado
    {
        [JsonProperty("idDepositante")]
        public int IdDepositante;

        [JsonProperty("depositante")]
        public string Depositante;

        [JsonProperty("areaComputaSaldo")]
        public bool AreaComputaSaldo;

        [JsonProperty("idProduto")]
        public int IdProduto;

        
        [JsonProperty("produtoNome")]
        [Description("Nome do Produto")]
        public string ProdutoNome;

        [JsonProperty("produtoCodigoInterno")]
        public string ProdutoCodigoInterno;

        [JsonProperty("produtoCodigoExterno")]
        public string ProdutoCodigoExterno;

        [JsonProperty("quantidade")]
        public int Quantidade;

        [JsonProperty("quantidadeProduto")]
        public int QuantidadeProduto;

        [JsonProperty("quantidadeDeMovimentacao")]
        public int QuantidadeDeMovimentacao;

        [JsonProperty("quantidadeProdutosEmbalagem")]
        public int QuantidadeProdutosEmbalagem;

        [JsonProperty("tipoUnidadeEmbalagem")]
        public string TipoUnidadeEmbalagem;

        [JsonProperty("tipoUnidadeMovimentacao")]
        public string TipoUnidadeMovimentacao;

        [JsonProperty("tipoUnidadeProduto")]
        public string TipoUnidadeProduto;

        [JsonProperty("quantidadeEnderecos")]
        public int QuantidadeEnderecos;

        [JsonProperty("quantidadeDisponivel")]
        public int QuantidadeDisponivel;

        [JsonProperty("quantidadeEmExpedicao")]
        public int QuantidadeEmExpedicao;

        [JsonProperty("pedidosCodigosExternos")]
        public List<string> PedidosCodigosExternos;

        [JsonProperty("imagens")]
        public List<object> Imagens;

        [JsonProperty("codigosDeIdentificacao")]
        public List<object> CodigosDeIdentificacao;

        [JsonProperty("notasFiscais")]
        public List<NotaFiscal> NotasFiscais;

        [JsonProperty("depositos")]
        public List<Deposito> Depositos;
    }
    
}
