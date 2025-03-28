using Newtonsoft.Json;

namespace LOGHouseSystem.Services.Tiny.Dto
{
    public class TinyCompleteOrderRequestDto
    {
        [JsonProperty("retorno")]
        public TinyOrderRetornoDto Retorno { get; set; }
    }

    public class TinyOrderRetornoDto
    {
        [JsonProperty("status_processamento")]
            
        public long StatusProcessamento { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("pedido")]
        public TinyOrderPedidoDto Pedido { get; set; }
    }

    public class TinyOrderPedidoDto
    {
        [JsonProperty("id")]
            
        public long Id { get; set; }

        [JsonProperty("numero")]
            
        public long Numero { get; set; }

        [JsonProperty("data_pedido")]
        public string DataPedido { get; set; }

        [JsonProperty("data_prevista")]
        public string DataPrevista { get; set; }

        [JsonProperty("data_faturamento")]
        public string DataFaturamento { get; set; }

        [JsonProperty("itens")]
        public TinyOrderItemListDto[] Itens { get; set; }

        [JsonProperty("condicao_pagamento")]
        public string CondicaoPagamento { get; set; }

        [JsonProperty("forma_pagamento")]
        public string FormaPagamento { get; set; }

        [JsonProperty("meio_pagamento")]
        public string MeioPagamento { get; set; }

        [JsonProperty("nome_transportador")]
        public string NomeTransportador { get; set; }

        [JsonProperty("frete_por_conta")]
        public string FretePorConta { get; set; }

        [JsonProperty("valor_frete")]
        public string ValorFrete { get; set; }

        [JsonProperty("valor_desconto")]
        public string ValorDesconto { get; set; }

        [JsonProperty("total_produtos")]
        public string TotalProdutos { get; set; }

        [JsonProperty("total_pedido")]
        public string TotalPedido { get; set; }

        [JsonProperty("numero_ordem_compra")]
            
        public long NumeroOrdemCompra { get; set; }

        [JsonProperty("deposito")]
        public string Deposito { get; set; }

        [JsonProperty("forma_envio")]
        public string FormaEnvio { get; set; }

        [JsonProperty("forma_frete")]
        public string FormaFrete { get; set; }

        [JsonProperty("situacao")]
        public string Situacao { get; set; }

        [JsonProperty("obs")]
        public string? Obs { get; set; }

        [JsonProperty("id_vendedor")]            
        public long IdVendedor { get; set; }

        [JsonProperty("nome_vendedor")]
        public string NomeVendedor { get; set; }

        [JsonProperty("codigo_rastreamento")]
        public string CodigoRastreamento { get; set; }

        [JsonProperty("url_rastreamento")]
        public string UrlRastreamento { get; set; }

        [JsonProperty("id_nota_fiscal")]
            
        public long IdNotaFiscal { get; set; }
        
        [JsonProperty("cliente")]            
        public TinyClientDto Cliente { get; set; }

        [JsonProperty("ecommerce")]
        public Ecommerce Ecommerce { get; set; }
    }

    public class TinyOrderItemListDto
    {
        [JsonProperty("item")]
        public TinyOrderItemDto Item { get; set; }

        // Internal rules
        public int? ProductId { get; set; } // LOGHouse Product Id
        public string? Gtin { get; set; } // Product Gtin used to save
    }

    public class TinyOrderItemDto
    {
        [JsonProperty("id_produto")]

        public long TinyId { get; set; }

        [JsonProperty("codigo")]
            
        public string Codigo { get; set; }

        [JsonProperty("descricao")]
        public string Descricao { get; set; }

        [JsonProperty("unidade")]
        public string Unidade { get; set; }

        [JsonProperty("quantidade")]
            
        public decimal Quantidade { get; set; }

        [JsonProperty("valor_unitario")]
        public decimal ValorUnitario { get; set; }
    }

    public partial class TinyClientDto
    {
        [JsonProperty("codigo")]        
        public string Codigo { get; set; }

        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("nome_fantasia")]
        public string NomeFantasia { get; set; }

        [JsonProperty("tipo_pessoa")]
        public string TipoPessoa { get; set; }

        [JsonProperty("cpf_cnpj")]
        public string CpfCnpj { get; set; }

        [JsonProperty("ie")]
        public string Ie { get; set; }

        [JsonProperty("rg")]        
        public string Rg { get; set; }

        [JsonProperty("endereco")]
        public string Endereco { get; set; }

        [JsonProperty("numero")]        
        public string Numero { get; set; }

        [JsonProperty("complemento")]
        public string Complemento { get; set; }

        [JsonProperty("bairro")]
        public string Bairro { get; set; }

        [JsonProperty("cep")]        
        public string Cep { get; set; }

        [JsonProperty("cidade")]
        public string Cidade { get; set; }

        [JsonProperty("uf")]
        public string Uf { get; set; }

        [JsonProperty("fone")]
        public string Fone { get; set; }
    }

    public class Ecommerce
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("numeroPedidoEcommerce")]
        public string NumeroPedidoEcommerce { get; set; }

        [JsonProperty("numeroPedidoCanalVenda")]        
        public string NumeroPedidoCanalVenda { get; set; }

        [JsonProperty("nomeEcommerce")]
        public string NomeEcommerce { get; set; }
    }
}
