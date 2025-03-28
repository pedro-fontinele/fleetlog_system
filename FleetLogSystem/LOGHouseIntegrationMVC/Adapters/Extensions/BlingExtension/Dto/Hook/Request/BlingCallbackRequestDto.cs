using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace LOGHouseSystem.Adapters.Extensions.BlingExtension.Dto.Hook.Request
{
    public class BlingCallbackRequestDto
    {
        public string data { get; set; }
    }

    public class BlingSituacaoPedidoCallbackRequest
    {
        [JsonProperty("retorno")]
        public BlingRetornoCallbackRequest Retorno;


        public bool IsStatusAtendido()
        {
            return Retorno.Pedidos[0].pedido.Situacao.ToLower() == "atendido";
        }

        public bool IsInvoiceSended()
        {
            return Retorno.Pedidos[0].pedido.Nota != null && (Retorno.Pedidos[0].pedido.Nota.Situacao == "7" || Retorno.Pedidos[0].pedido.Nota.Situacao == "6");
        }
    }

    public class BlingRetornoCallbackRequest
    {
        [JsonProperty("pedidos")]
        public List<BlingPedidoListCallbackRequest> Pedidos;

        [JsonProperty("erros")]
        public List<BlingErrosListCallbackRequest> Erros;
    }

    public class BlingErrosListCallbackRequest
    {
        [JsonProperty("erro")]
        public BlingErroListCallbackRequest Erro { get; set; }
    }

    public class BlingErroListCallbackRequest
    {
        [JsonProperty("cod")]
        public int Cod { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }
    }  


    public class BlingItemCallbackRequest
    {
        [JsonProperty("codigo")]
        public string Codigo;

        [JsonProperty("descricao")]
        public string Descricao;

        [JsonProperty("quantidade")]
        public decimal Quantidade;

        [JsonProperty("valorunidade")]
        public decimal Valorunidade;

        [JsonProperty("precocusto")]
        public string Precocusto;

        [JsonProperty("descontoItem")]
        public string DescontoItem;

        [JsonProperty("un")]
        public string Un;

        [JsonProperty("pesoBruto")]
        public string PesoBruto;

        [JsonProperty("largura")]
        public string Largura;

        [JsonProperty("altura")]
        public string Altura;

        [JsonProperty("profundidade")]
        public string Profundidade;

        [JsonProperty("descricaoDetalhada")]
        public string DescricaoDetalhada;

        [JsonProperty("unidadeMedida")]
        public string UnidadeMedida;

        [JsonProperty("gtin")]
        public string Gtin;
    }

    public class BlingItenCallbackRequest
    {
        [JsonProperty("item")]
        public BlingItemCallbackRequest Item;
    }

    public class BlingNotaCallbackRequest
    {
        [JsonProperty("serie")]
        public string Serie;

        [JsonProperty("numero")]
        public string Numero;

        [JsonProperty("dataEmissao")]
        public DateTime DataEmissao;

        [JsonProperty("situacao")]
        public string Situacao;

        [JsonProperty("valorNota")]
        public float ValorNota;

        [JsonProperty("chaveAcesso")]
        public string ChaveAcesso;
    }

    public class BlingPedidoListCallbackRequest
    {
        [JsonProperty("pedido")]
        public BlingPedidoCallbackRequest pedido;
    }

    public class BlingPedidoCallbackRequest
    {
        [JsonProperty("desconto")]
        public string Desconto;

        [JsonProperty("observacoes")]
        public string Observacoes;

        [JsonProperty("observacaointerna")]
        public string Observacaointerna;

        [JsonProperty("data")]
        public string Data;

        [JsonProperty("numero")]
        public string Numero;

        [JsonProperty("numeroOrdemCompra")]
        public string NumeroOrdemCompra;

        [JsonProperty("vendedor")]
        public string Vendedor;

        [JsonProperty("valorfrete")]
        public string Valorfrete;

        [JsonProperty("outrasdespesas")]
        public string Outrasdespesas;

        [JsonProperty("totalprodutos")]
        public string Totalprodutos;

        [JsonProperty("totalvenda")]
        public string Totalvenda;

        [JsonProperty("situacao")]
        public string Situacao;

        [JsonProperty("loja")]
        public string Loja;

        [JsonProperty("numeroPedidoLoja")]
        public string NumeroPedidoLoja;

        [JsonProperty("tipoIntegracao")]
        public string TipoIntegracao;

        [JsonProperty("nota")]
        public BlingNotaCallbackRequest Nota;

        [JsonProperty("intermediador")]
        public BlingIntermediadorCallbackRequest Intermediador; // This fiels isn't in Bling documentation

        [JsonProperty("itens")]
        public List<BlingItenCallbackRequest> Itens;

        [JsonProperty("transporte")]
        public BlingTransporteCallbackRequest Transporte;

        [JsonProperty("cliente")]
        public BlingClientCallbackRequest Client;
    }

    public class BlingTransporteCallbackRequest
    {
        [JsonProperty("enderecoEntrega")]
        public BlingEnderecoEntregaCallbackRequest EnderecoEntrega { get; set; }

        [JsonProperty("volumes")]
        public List<BlingVolumesCallbackRequest> Volumes { get; set; }
    }

    public class BlingVolumesCallbackRequest
    {
        [JsonProperty("volume")]
        public BlingVolumeCallbackRequest Volume { get; set; }        
    }

    public class BlingVolumeCallbackRequest
    {
        [JsonProperty("codigoRastreamento")]
        public string CodigoRastreamento { get; set; }

        [JsonProperty("servico")]
        public string Servico { get; set; }
    }

    public class BlingEnderecoEntregaCallbackRequest
    {
        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("endereco")]
        public string Endereco { get; set; }

        [JsonProperty("numero")]
        public string Numero { get; set; }

        [JsonProperty("complemento")]
        public string Complemento { get; set; }

        [JsonProperty("cidade")]
        public string Cidade { get; set; }

        [JsonProperty("bairro")]
        public string Bairro { get; set; }

        [JsonProperty("cep")]
        public string Cep { get; set; }

        [JsonProperty("uf")]
        public string Uf { get; set; }

    }

    public class BlingIntermediadorCallbackRequest
    {
        [JsonProperty("cnpj")]
        public string Cnpj { get; set; }

        [JsonProperty("nomeUsuario")]
        public string NomeUsuario { get; set; }
    }

    public class BlingClientCallbackRequest
    {
        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("cnpj")]
        public string Cnpj { get; set; }

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

        [JsonProperty("cidade")]
        public string Cidade { get; set; }

        [JsonProperty("bairro")]
        public string Bairro { get; set; }

        [JsonProperty("cep")]
        public string Cep { get; set; }

        [JsonProperty("uf")]
        public string Uf { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("celular")]
        public string Celular { get; set; }

        [JsonProperty("fone")]
        public string Fone { get; set; }
    }
}
