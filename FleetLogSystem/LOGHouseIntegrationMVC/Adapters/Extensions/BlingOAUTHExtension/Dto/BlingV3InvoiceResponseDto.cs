using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LOGHouseSystem.Adapters.Extensions.BlingOAUTHExtension.Dto
{
    public class Endereco
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

    public class Contato
    {
        public int id { get; set; }
        public string nome { get; set; }
        public string numeroDocumento { get; set; }
        public string ie { get; set; }
        public string rg { get; set; }
        public string telefone { get; set; }
        public string email { get; set; }
        public Endereco endereco { get; set; }
    }

    public class NaturezaOperacao
    {
        public int id { get; set; }
    }

    public class Loja
    {
        public int id { get; set; }
    }

    public class Data
    {
        public int id { get; set; }
        public int tipo { get; set; }
        public int situacao { get; set; }
        public string numero { get; set; }
        public DateTime dataEmissao { get; set; }
        public DateTime dataOperacao { get; set; }
        public Contato contato { get; set; }
        public NaturezaOperacao naturezaOperacao { get; set; }
        public Loja loja { get; set; }
    }

    public class BlingV3InvoiceResponseDto
    {
        public List<Data> data { get; set; }
    }
}