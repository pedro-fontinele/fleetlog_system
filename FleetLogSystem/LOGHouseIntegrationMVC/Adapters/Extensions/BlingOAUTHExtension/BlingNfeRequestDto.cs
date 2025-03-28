using Newtonsoft.Json;

namespace LOGHouseSystem.Adapters.Extensions.BlingOAUTHExtension
{
    public class BlingNfeRequestDto
    {
        [JsonProperty("tipo")]
        public int Tipo { get; set; }

        [JsonProperty("dataOperacao")]
        public string DataOperacao { get; set; }

        [JsonProperty("contato")]
        public BlingNfeContatoRequestDto Contato { get; set; }

        [JsonProperty("naturezaOperacao")]
        public BlingNfeNaturezaOperacaoRequestDto NaturezaOperacao { get; set; }

        [JsonProperty("finalidade")]
        public int Finalidade { get; set; }

        [JsonProperty("seguro")]
        public decimal Seguro { get; set; }

        [JsonProperty("despesas")]
        public decimal Despesas { get; set; }

        [JsonProperty("desconto")]
        public decimal Desconto { get; set; }
         
        [JsonProperty("observacoes")]
        public string Observacoes { get; set; }

        [JsonProperty("serie")]
        public int Serie { get; set; }

        [JsonProperty("itens")]
        public List<BlingNfeItensRequestDto> Items { get; set; }
    }

    public class BlingNfeItensRequestDto
    {
        [JsonProperty("codigo")]
        public string Codigo { get; set; }

        [JsonProperty("descricao")]
        public string Descricao { get; set; }

        [JsonProperty("unidade")]
        public string Unidade { get; set; }

        [JsonProperty("quantidade")]
        public decimal Quantidade { get; set; }

        [JsonProperty("valor")]
        public decimal Valor { get; set; }

        [JsonProperty("tipo")]
        public string Tipo { get; set; }

        [JsonProperty("pesoBruto")]
        public decimal PesoBruto { get; set; }

        [JsonProperty("pesoLiquido")]
        public decimal PesoLiquido { get; set; }

        [JsonProperty("cest")]
        public string Cest { get; set; }

        [JsonProperty("origem")]
        public int Origem { get; set; }

        [JsonProperty("informacoesAdicionais")]
        public string InformacoesAdicionais { get; set; }

        [JsonProperty("numeroPedidoCompra")]
        public string NumeroPedidoCompra { get; set; }

        [JsonProperty("classificacaoFiscal")]
        public string ClassificacaoFiscal { get; set; }

        [JsonProperty("codigoServico")]
        public string CodigoServico { get; set; }

        [JsonProperty("numero")]
        public string Numero { get; set; }
    }

    public class BlingNfeNaturezaOperacaoRequestDto
    {
        [JsonProperty("id")]
        public long Id { get; set; }        
    }

    public class BlingNfeContatoRequestDto
    {
        [JsonProperty("nome")]
        public string Nome { get; set; }
        [JsonProperty("tipoPessoa")]
        public string TipoPessoa { get; set; }

        [JsonProperty("numeroDocumento")]
        public string NumeroDocumento { get; set; }

        [JsonProperty("ie")]
        public string IE { get; set; }

        [JsonProperty("RG")]
        public string RG { get; set; }

        [JsonProperty("contribuinte")]
        public int Contribuinte { get; set; }

        [JsonProperty("endereco")]
        public BlingNfeEnderecoRequestDto Endereco { get; set; }
    }


    public class BlingNfeEnderecoRequestDto
    {
        [JsonProperty("endereco")]
        public string Endereco { get; set; }

        [JsonProperty("numero")]
        public string? Numero { get; set; }

        [JsonProperty("complemento")]
        public string Complemento { get; set; }

        [JsonProperty("bairro")]
        public string Bairro { get; set; }

        [JsonProperty("cep")]
        public string Cep { get; set; }

        [JsonProperty("municipio")]
        public string Municio { get; set; }

        [JsonProperty("uf")]
        public string UF { get; set; }

        [JsonProperty("pais")]
        public string Pais { get; set; }
    }
}

