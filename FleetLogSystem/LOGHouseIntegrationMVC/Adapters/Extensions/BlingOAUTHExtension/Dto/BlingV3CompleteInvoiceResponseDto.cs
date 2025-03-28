using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LOGHouseSystem.Adapters.Extensions.BlingOAUTHExtension.Dto
{
    public class BlingV3Endereco
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

    public class BlingV3Contato
    {
        public int id { get; set; }
        public string nome { get; set; }
        public string numeroDocumento { get; set; }
        public string ie { get; set; }
        public string rg { get; set; }
        public string telefone { get; set; }
        public string email { get; set; }
        public BlingV3Endereco endereco { get; set; }
    }

    public class BlingV3NaturezaOperacao
    {
        public int id { get; set; }
    }

    public class BlingV3Loja
    {
        public int id { get; set; }
    }

    public class BlingV3Etiqueta
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

    public class BlingV3Volume
    {
        public int id { get; set; }
    }

    public class BlingV3Transportador
    {
        public string nome { get; set; }
        public string numeroDocumento { get; set; }
    }

    public class BlingV3Transporte
    {
        public int fretePorConta { get; set; }
        public BlingV3Transportador transportador { get; set; }
        public List<BlingV3Volume> volumes { get; set; }
        public BlingV3Etiqueta etiqueta { get; set; }
    }

    public class BlingV3Data
    {
        public int id { get; set; }
        public int tipo { get; set; }
        public int situacao { get; set; }
        public string numero { get; set; }
        public DateTime dataEmissao { get; set; }
        public DateTime dataOperacao { get; set; }
        public BlingV3Contato contato { get; set; }
        public BlingV3NaturezaOperacao naturezaOperacao { get; set; }
        public BlingV3Loja loja { get; set; }
        public int serie { get; set; }
        public string chaveAcesso { get; set; }
        public string xml { get; set; }
        public string linkDanfe { get; set; }
        public string linkPDF { get; set; }
        public BlingV3Transporte transporte { get; set; }
    }

    public class BlingV3CompleteInvoiceResponseDto
    {
        public BlingV3Data data { get; set; }
    }
}