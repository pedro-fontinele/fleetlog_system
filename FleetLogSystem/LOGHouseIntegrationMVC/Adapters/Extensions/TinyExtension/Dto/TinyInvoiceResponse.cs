namespace LOGHouseSystem.Adapters.Extensions.TinyExtension.Dto
{
    public class TinyInvoiceStatusProcessamento
    {
        public int status_processamento { get; set; }
        public string status { get; set; }
        public int? codigo_erro { get; set; }
        public List<TinyInvoiceErro> erros { get; set; }
        public TinyInvoiceNotaFiscal nota_fiscal { get; set; }
    }

    public class TinyInvoiceErro
    {
        public string erro { get; set; }
    }

    public class TinyInvoiceNotaFiscal
    {
        public long id { get; set; }
        public string tipo_nota { get; set; }
        public string natureza_operacao { get; set; }
        public int regime_tributario { get; set; }
        public int finalidade { get; set; }
        public int serie { get; set; }
        public long numero { get; set; }
        public string numero_ecommerce { get; set; }
        public string data_emissao { get; set; }
        public string data_saida { get; set; }
        public string hora_saida { get; set; }
        public TinyInvoiceCliente cliente { get; set; }
        public TinyInvoiceEndereco endereco_entrega { get; set; }
        public List<TinyInvoiceItem> itens { get; set; }
        public decimal? base_icms { get; set; }
        public decimal? valor_icms { get; set; }
        public decimal? base_icms_st { get; set; }
        public decimal? valor_icms_st { get; set; }
        public decimal? valor_servicos { get; set; }
        public decimal? valor_produtos { get; set; }
        public decimal? valor_frete { get; set; }
        public decimal? valor_seguro { get; set; }
        public decimal? valor_outras { get; set; }
        public decimal? valor_ipi { get; set; }
        public decimal? valor_issqn { get; set; }
        public decimal? valor_nota { get; set; }
        public decimal? valor_desconto { get; set; }
        public decimal? valor_faturado { get; set; }
        public string frete_por_conta { get; set; }
        public TinyInvoiceTransportador transportador { get; set; }
        public int? quantidade_volumes { get; set; }
        public string especie_volumes { get; set; }
        public string marca_volumes { get; set; }
        public string numero_volumes { get; set; }
        public decimal? peso_bruto { get; set; }
        public decimal? peso_liquido { get; set; }
        public TinyInvoiceFormaEnvio forma_envio { get; set; }
        public TinyInvoiceFormaFrete forma_frete { get; set; }
        public string codigo_rastreamento { get; set; }
        public string url_rastreamento { get; set; }
        public string forma_pagamento { get; set; }
        public string meio_pagamento { get; set; }
        public string condicao_pagamento { get; set; }
        public List<TinyInvoiceParcela> parcelas { get; set; }
        public int? id_venda { get; set; }
        public int? id_vendedor { get; set; }
        public string nome_vendedor { get; set; }
        public int? situacao { get; set; }
        public string descricao_situacao { get; set; }
        public string obs { get; set; }
        public string chave_acesso { get; set; }
        public List<TinyInvoiceMarcador> marcadores { get; set; }
        public TinyInvoiceIntermediador intermediador { get; set; }
    }

    public class TinyInvoiceCliente
    {
        public string nome { get; set; }
        public string tipo_pessoa { get; set; }
        public string cpf_cnpj { get; set; }
        public string ie { get; set; }
        public string endereco { get; set; }
        public string numero { get; set; }
        public string complemento { get; set; }
        public string bairro { get; set; }
        public string cep { get; set; }
        public string cidade { get; set; }
        public string uf { get; set; }
        public string fone { get; set; }
        public string email { get; set; }
    }

    public class TinyInvoiceEndereco
    {
        public string tipo_pessoa { get; set; }
        public string cpf_cnpj { get; set; }
        public string endereco { get; set; }
        public string numero { get; set; }
        public string complemento { get; set; }
        public string bairro { get; set; }
        public string cep { get; set; }
        public string cidade { get; set; }
        public string uf { get; set; }
        public string fone { get; set; }
        public string nome_destinatario { get; set; }
        public string ie { get; set; }
    }

    public class TinyInvoiceItem
    {
        public TinyInvoiceItemProduto produto { get; set; }
        public decimal id_produto { get; set; }
        public string codigo { get; set; }
        public string descricao { get; set; }
        public string unidade { get; set; }
        public string ncm { get; set; }
        public decimal quantidade { get; set; }
        public decimal valor_unitario { get; set; }
        public decimal valor_total { get; set; }
        public string cfop { get; set; }
        public string natureza { get; set; }
    }

    public class TinyInvoiceItemProduto
    {
        public long id { get; set; }
    }

    public class TinyInvoiceTransportador
    {
        public string nome { get; set; }
        public string cpf_cnpj { get; set; }
        public string ie { get; set; }
        public string endereco { get; set; }
        public string cidade { get; set; }
        public string uf { get; set; }
        public string placa { get; set; }
        public string uf_placa { get; set; }
    }

    public class TinyInvoiceFormaEnvio
    {
        public int id { get; set; }
        public string descricao { get; set; }
    }

    public class TinyInvoiceFormaFrete
    {
        public int id { get; set; }
        public string descricao { get; set; }
    }

    public class TinyInvoiceParcela
    {
        public int dias { get; set; }
        public string data { get; set; }
        public decimal valor { get; set; }
        public string obs { get; set; }
        public string forma_pagamento { get; set; }
        public string meio_pagamento { get; set; }
    }

    public class TinyInvoiceMarcador
    {
        public int id { get; set; }
        public string descricao { get; set; }
        public string cor { get; set; }
    }

    public class TinyInvoiceIntermediador
    {
        public string nome { get; set; }
        public string cnpj { get; set; }
        public string cnpjPagamento { get; set; }
    }

    public class TinyInvoiceResponse
    {
        public TinyInvoiceStatusProcessamento retorno { get; set; }
    }
}
