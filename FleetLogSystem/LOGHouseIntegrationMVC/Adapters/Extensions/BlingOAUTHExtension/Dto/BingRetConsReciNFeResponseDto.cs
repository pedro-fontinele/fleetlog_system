using System.Xml.Serialization;

namespace LOGHouseSystem.Adapters.Extensions.BlingOAUTHExtension.Dto
{
    [XmlRoot(ElementName = "infProt")]
    public class InfProt
    {

        [XmlElement(ElementName = "tpAmb")]
        public int TpAmb { get; set; }

        [XmlElement(ElementName = "verAplic")]
        public string VerAplic { get; set; }

        [XmlElement(ElementName = "chNFe")]
        public string ChNFe { get; set; }

        [XmlElement(ElementName = "dhRecbto")]
        public DateTime DhRecbto { get; set; }

        [XmlElement(ElementName = "nProt")]
        public string NProt { get; set; }

        [XmlElement(ElementName = "digVal")]
        public string DigVal { get; set; }

        [XmlElement(ElementName = "cStat")]
        public string CStat { get; set; }

        [XmlElement(ElementName = "xMotivo")]
        public string XMotivo { get; set; }
    }

    [XmlRoot(ElementName = "protNFe")]
    public class ProtNFe
    {

        [XmlElement(ElementName = "infProt")]
        public InfProt InfProt { get; set; }

        [XmlAttribute(AttributeName = "versao")]
        public string Versao { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "retConsReciNFe", Namespace = "http://www.portalfiscal.inf.br/nfe", IsNullable = true)]
    public class BingRetConsReciNFeResponseDto
    {

        [XmlElement(ElementName = "tpAmb")]
        public int TpAmb { get; set; }

        [XmlElement(ElementName = "verAplic")]
        public string VerAplic { get; set; }

        [XmlElement(ElementName = "nRec")]
        public double NRec { get; set; }

        [XmlElement(ElementName = "cStat")]
        public int CStat { get; set; }

        [XmlElement(ElementName = "xMotivo")]
        public string XMotivo { get; set; }

        [XmlElement(ElementName = "cUF")]
        public int CUF { get; set; }

        [XmlElement(ElementName = "dhRecbto")]
        public DateTime DhRecbto { get; set; }

        [XmlElement(ElementName = "protNFe")]
        public ProtNFe ProtNFe { get; set; }

        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }

        [XmlAttribute(AttributeName = "versao")]
        public string Versao { get; set; }
    }
}
