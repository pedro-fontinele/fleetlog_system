using Newtonsoft.Json;

namespace LOGHouseSystem.Adapters.Extensions.BlingExtension.Dto.Hook.Request
{
    public partial class BlingInvoiceDataResponseDto
    {
        [JsonProperty("retorno")]
        public BlingInvoiceDataRetornoResponseDto Retorno { get; set; }

        public bool IsInvoiceSended()
        {
            return Retorno.Notasfiscais[0].Notafiscal.Situacao == "Autorizada" || Retorno.Notasfiscais[0].Notafiscal.Situacao == "Emitida DANFE";
        }

        public bool IsInvoiceCanceled()
        {
            List<string> listCanceledSituacoes = new List<string>() { 
                "Cancelada", 
                //"Rejeitada", 
                "Denegada", 
                "Bloqueada" };

            return listCanceledSituacoes.Any(e => e == Retorno.Notasfiscais[0].Notafiscal.Situacao);
        }
    }

    public partial class BlingInvoiceDataRetornoResponseDto
    {
        [JsonProperty("notasfiscais")]
        public BlingInvoiceDataNotasFiscaisResponseDto[] Notasfiscais { get; set; }
    }

    public partial class BlingInvoiceDataNotasFiscaisResponseDto
    {
        [JsonProperty("notafiscal")]
        public BlingInvoiceDataNotaFiscalResponseDto Notafiscal { get; set; }
    }

    public partial class BlingInvoiceDataNotaFiscalResponseDto
    {
        [JsonProperty("serie")]        
        public long Serie { get; set; }

        [JsonProperty("numero")]
        public string Numero { get; set; }

        [JsonProperty("numeroPedidoLoja")]        
        public string NumeroPedidoLoja { get; set; }

        [JsonProperty("loja")]
        public long Loja { get; set; }

        [JsonProperty("tipo")]
        public string Tipo { get; set; }

        [JsonProperty("situacao")]
        public string Situacao { get; set; }

        [JsonProperty("contato")]
        public string Contato { get; set; }

        [JsonProperty("cliente")]
        public BlingInvoiceDataClienteResponseDto Cliente { get; set; }

        [JsonProperty("cnpj")]
        public string Cnpj { get; set; }

        [JsonProperty("vendedor")]
        public string Vendedor { get; set; }

        [JsonProperty("dataEmissao")]
        public DateTimeOffset DataEmissao { get; set; }

        [JsonProperty("valorNota")]
        public decimal ValorNota { get; set; }

        [JsonProperty("chaveAcesso")]
        public string ChaveAcesso { get; set; }

        [JsonProperty("xml")]
        public string Xml { get; set; }

        [JsonProperty("linkDanfe")]
        public string LinkDanfe { get; set; }

        [JsonProperty("cfops")]
        public long[] Cfops { get; set; }

        [JsonProperty("transporte")]
        public BlingInvoiceDataTransporteResponseDto Transporte { get; set; }
    }

    public partial class BlingInvoiceDataClienteResponseDto
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

        [JsonProperty("fone")]
        public string Fone { get; set; }
    }

    public partial class BlingInvoiceDataTransporteResponseDto
    {
        [JsonProperty("transportadora")]
        public string Transportadora { get; set; }

        [JsonProperty("cnpj")]
        public string Cnpj { get; set; }

        [JsonProperty("tipo_frete")]
        public string TipoFrete { get; set; }

        [JsonProperty("volumes")]
        public BlingInvoiceDataVolumeElementResponseDto[] Volumes { get; set; }

        [JsonProperty("enderecoEntrega")]
        public BlingInvoiceDataEnderecoEntregaResponseDto EnderecoEntrega { get; set; }
    }

    public partial class BlingInvoiceDataEnderecoEntregaResponseDto
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

    public partial class BlingInvoiceDataVolumeElementResponseDto
    {
        [JsonProperty("volume")]
        public BlingInvoiceDataVolumeVolumeResponseDto Volume { get; set; }
    }

    public partial class BlingInvoiceDataVolumeVolumeResponseDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("idServico")]
        public string IdServico { get; set; }

        [JsonProperty("servico")]
        public string Servico { get; set; }

        [JsonProperty("codigoServico")]
        public string CodigoServico { get; set; }

        [JsonProperty("codigoRastreamento")]
        public string CodigoRastreamento { get; set; }

        [JsonProperty("dataSaida")]
        public DateTimeOffset DataSaida { get; set; }

        [JsonProperty("prazoEntregaPrevisto")]
        public string PrazoEntregaPrevisto { get; set; }

        [JsonProperty("valorFretePrevisto")]
        public string ValorFretePrevisto { get; set; }

        [JsonProperty("valorDeclarado")]
        public string ValorDeclarado { get; set; }

        [JsonProperty("remessa")]
        public BlingInvoiceDataRemessaResponseDto Remessa { get; set; }

        [JsonProperty("dimensoes")]
        public BlingInvoiceDataDimensoesResponseDto Dimensoes { get; set; }

        [JsonProperty("urlRastreamento", NullValueHandling = NullValueHandling.Ignore)]
        public Uri UrlRastreamento { get; set; }
    }

    public partial class BlingInvoiceDataDimensoesResponseDto
    {
        [JsonProperty("peso")]
        public string Peso { get; set; }

        [JsonProperty("altura")]
        public long Altura { get; set; }

        [JsonProperty("largura")]
        public long Largura { get; set; }

        [JsonProperty("comprimento")]
        public long Comprimento { get; set; }

        [JsonProperty("diametro")]
        public long Diametro { get; set; }
    }

    public partial class BlingInvoiceDataRemessaResponseDto
    {
        [JsonProperty("numero")]
        public string Numero { get; set; }

        [JsonProperty("dataCriacao")]
        public string DataCriacao { get; set; }
    }
}
