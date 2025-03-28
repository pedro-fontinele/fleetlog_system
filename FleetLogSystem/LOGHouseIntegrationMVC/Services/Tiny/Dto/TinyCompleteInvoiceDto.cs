using LOGHouseSystem.Adapters.Extensions.NFeExtension;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace LOGHouseSystem.Services.Tiny.Dto
{    
    public class TinyCompleteInvoiceDto
    {
        [XmlElement(ElementName = "retorno")]
        public TinyRetornoInvoiceDto Retorno { get; set; }
    }

    public class TinyRetornoInvoiceDto
    {
        [XmlElement(ElementName = "status_procesamento")]
        public long StatusProcessamento { get; set; }

        [XmlElement(ElementName = "status")]
        public string Status { get; set; }

        [XmlElement(ElementName = "codigo_erro")]
        public int CodigoErro { get; set; }

        [XmlElement(ElementName = "erros")]
        public List<TinyErrosInvoiceDto> Erros { get; set; }

        [XmlElement(ElementName = "nfe_xml")]
        public NfeProc XmlNfe { get; set; }
    }


    [XmlRoot(ElementName = "erros")]
    public class TinyErrosInvoiceDto
    {
        [XmlElement(ElementName = "erro")]
        public string Erro { get; set; }
            
    }
    
}
