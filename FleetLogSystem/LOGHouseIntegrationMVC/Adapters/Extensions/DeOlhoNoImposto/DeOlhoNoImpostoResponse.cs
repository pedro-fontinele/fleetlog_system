namespace LOGHouseSystem.Adapters.Extensions.DeOlhoNoImposto
{
    public class DeOlhoNoImpostoResponse
    {
        public string Codigo { get; set; }
        public string UF { get; set; }
        public int EX { get; set; }
        public string Descricao { get; set; }
        public decimal Nacional { get; set; }
        public decimal Estadual { get; set; }
        public decimal Importado { get; set; }
        public decimal Municipal { get; set; }
        public string Tipo { get; set; }
        public string VigenciaInicio { get; set; }
        public string VigenciaFim { get; set; }
        public string Chave { get; set; }
        public string Versao { get; set; }
        public string Fonte { get; set; }
    }
}
