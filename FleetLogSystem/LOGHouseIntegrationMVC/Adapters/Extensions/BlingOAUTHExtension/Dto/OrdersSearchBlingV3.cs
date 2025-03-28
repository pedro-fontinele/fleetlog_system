namespace LOGHouseSystem.Adapters.Extensions.BlingOAUTHExtension.Dto
{
    public class OrdersSearchBlingV3
    {
        public List<OrderDataBlingV3> Data { get; set; }
    }

    public class OrderDataBlingV3
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public string NumeroLoja { get; set; }
        public DateTime Data { get; set; }
        public DateTime DataSaida { get; set; }
        public DateTime DataPrevista { get; set; }
        public int TotalProdutos { get; set; }
        public int Total { get; set; }
        public ContactBlingV3 Contato { get; set; }
        public SituationBlingV3 Situacao { get; set; }
        public StoreBlingV3 Loja { get; set; }
    }

    public class ContactBlingV3
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string TipoPessoa { get; set; }
        public string NumeroDocumento { get; set; }
    }

    public class SituationBlingV3
    {
        public int Id { get; set; }
        public int Valor { get; set; }
    }

    public class StoreBlingV3
    {
        public int Id { get; set; }
    }
}
