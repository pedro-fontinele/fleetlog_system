namespace LOGHouseSystem.Adapters.Extensions.BlingOAUTHExtension.Dto
{
    public class BlingV3GetInvoiceResponse
    {
        public BlingV3GetInvoiceData data { get; set; }
    }

    public class BlingV3GetInvoiceData
    {
        public long id { get; set; }
        public int tipo { get; set; }
        public int situacao { get; set; }
        public int numero { get; set; }
        public string dataEmissao { get; set; }
        public string dataOperacao { get; set; }
        public BlingV3GetInvoiceContato contato { get; set; }
        public BlingV3GetInvoiceNaturezaOperacao naturezaOperacao { get; set; }
        public BlingV3GetInvoiceLoja loja { get; set; }
        public int serie { get; set; }
        public string chaveAcesso { get; set; }
        public string xml { get; set; }
        public string linkDanfe { get; set; }
        public string linkPDF { get; set; }
        public BlingV3GetInvoiceTransporte transporte { get; set; }
    }

    public class BlingV3GetInvoiceContato
    {
        public long id { get; set; }
        public string nome { get; set; }
        public string numeroDocumento { get; set; }
        public string ie { get; set; }
        public string rg { get; set; }
        public string telefone { get; set; }
        public string email { get; set; }
        public BlingV3GetInvoiceEndereco endereco { get; set; }
    }

    public class BlingV3GetInvoiceEndereco
    {
        public string endereco { get; set; }
        public string numero { get; set; }
        public string complemento { get; set; }
        public string bairro { get; set; }
        public string cep { get; set; }
        public string municipio { get; set; }
        public string uf { get; set; }
        public string pais { get; set; }
    }

    public class BlingV3GetInvoiceNaturezaOperacao
    {
        public long id { get; set; }
    }

    public class BlingV3GetInvoiceLoja
    {
        public long id { get; set; }
    }

    public class BlingV3GetInvoiceTransporte
    {
        public int fretePorConta { get; set; }
        public BlingV3GetInvoiceTransportador transportador { get; set; }
        public List<BlingV3GetInvoiceVolume> volumes { get; set; }
        public BlingV3GetInvoiceEtiqueta etiqueta { get; set; }
    }

    public class BlingV3GetInvoiceTransportador
    {
        public string nome { get; set; }
        public string numeroDocumento { get; set; }
    }

    public class BlingV3GetInvoiceVolume
    {
        public long id { get; set; }
    }

    public class BlingV3GetInvoiceEtiqueta
    {
        public string nome { get; set; }
        public string endereco { get; set; }
        public string numero { get; set; }
        public string complemento { get; set; }
        public string municipio { get; set; }
        public string uf { get; set; }
        public string cep { get; set; }
        public string bairro { get; set; }
    }
}
