namespace LOGHouseSystem.Adapters.Extensions.BlingOAUTHExtension.Dto
{    
    public class CategoriaBlingV3
    {
        public int id { get; set; }
    }

    public class ComissaoBlingV3
    {
        public int @base { get; set; }
        public int aliquota { get; set; }
        public double valor { get; set; }
    }

    public class ContatoBlingV3
    {
        public int id { get; set; }
        public string tipoPessoa { get; set; }
        public string numeroDocumento { get; set; }
        public string nome { get; set; }
    }

    public class DescontoBlingV3
    {
        public double valor { get; set; }
        public string unidade { get; set; }
    }

    public class EtiquetaBlingV3
    {
        public string nome { get; set; }
        public string endereco { get; set; }
        public string numero { get; set; }
        public string complemento { get; set; }
        public string municipio { get; set; }
        public string uf { get; set; }
        public string cep { get; set; }
        public string bairro { get; set; }
        public string nomePais { get; set; }
    }

    public class FormaPagamentoBlingV3
    {
        public int id { get; set; }
    }

    public class IntermediadorBlingV3
    {
        public string cnpj { get; set; }
        public string nomeUsuario { get; set; }
    }

    public class ItenBlingV3
    {
        public int id { get; set; }
        public string codigo { get; set; }
        public string unidade { get; set; }
        public int quantidade { get; set; }
        public double desconto { get; set; }
        public double valor { get; set; }
        public int aliquotaIPI { get; set; }
        public string descricao { get; set; }
        public string descricaoDetalhada { get; set; }
        public ProdutoBlingV3 produto { get; set; }
        public ComissaoBlingV3 comissao { get; set; }
    }

    public class LojaBlingV3
    {
        public int id { get; set; }
    }

    public class ParcelaBlingV3
    {
        public int id { get; set; }
        public string dataVencimento { get; set; }
        public double valor { get; set; }
        public string observacoes { get; set; }
        public FormaPagamentoBlingV3 formaPagamento { get; set; }
    }

    public class ProdutoBlingV3
    {
        public int id { get; set; }
    }

    public class OrderBlingV3
    {
        public int numero { get; set; }
        public string numeroLoja { get; set; }
        public string data { get; set; }
        public string dataSaida { get; set; }
        public string dataPrevista { get; set; }
        public ContatoBlingV3 contato { get; set; }
        public LojaBlingV3 loja { get; set; }
        public string numeroPedidoCompra { get; set; }
        public int outrasDespesas { get; set; }
        public string observacoes { get; set; }
        public decimal totalProdutos { get; set; }
        public decimal total { get; set; }
        public string observacoesInternas { get; set; }
        public DescontoBlingV3 desconto { get; set; }
        public CategoriaBlingV3 categoria { get; set; }
        public TributacaoBlingV3 tributacao { get; set; }
        public List<ItenBlingV3> itens { get; set; }
        public List<ParcelaBlingV3> parcelas { get; set; }
        public TransporteBlingV3 transporte { get; set; }
        public VendedorBlingV3 vendedor { get; set; }
        public IntermediadorBlingV3 intermediador { get; set; }
        public TaxasBlingV3 taxas { get; set; }
        public SituacaoBlingV3 situacao { get; set; }
    }

    public class TaxasBlingV3
    {
        public int taxaComissao { get; set; }
        public double custoFrete { get; set; }
        public double valorBase { get; set; }
    }

    public class TransporteBlingV3
    {
        public double fretePorConta { get; set; }
        public double frete { get; set; }
        public int quantidadeVolumes { get; set; }
        public double pesoBruto { get; set; }
        public int prazoEntrega { get; set; }
        public ContatoBlingV3 contato { get; set; }
        public EtiquetaBlingV3 etiqueta { get; set; }
        public List<VolumeBlingV3> volumes { get; set; }
    }

    public class TributacaoBlingV3
    {
        public double totalICMS { get; set; }
        public double totalIPI { get; set; }
    }

    public class VendedorBlingV3
    {
        public int id { get; set; }
    }

    public class VolumeBlingV3
    {
        public int id { get; set; }
        public string servico { get; set; }
        public string codigoRastreamento { get; set; }
    }

    public class SituacaoBlingV3
    {
        public int id { get; set; }
        public string valor { get; set; }
    }
}