using System.Xml.Serialization;

namespace LOGHouseSystem.Adapters.Extensions.NFeExtension
{

    [XmlRoot(ElementName = "ide")]
    public class Ide
    {

        [XmlElement(ElementName = "serie")]
        public int Serie { get; set; }

        [XmlElement(ElementName = "nNF")]
        public int NNF { get; set; }

        [XmlElement(ElementName = "dhEmi")]
        public DateTime DhEmi { get; set; }

    }

    [XmlRoot(ElementName = "emit")]
    public class Emit
    {

        [XmlElement(ElementName = "CNPJ")]
        public string? CNPJ { get; set; }

        [XmlElement(ElementName = "xNome")]
        public string? XNome { get; set; }

        [XmlElement(ElementName = "enderEmit")]
        public Ender Ender { get; set; }

        [XmlElement(ElementName = "IE")]
        public string? IE { get; set; }
    }

    [XmlRoot(ElementName = "dest")]
    public class Dest
    {

        [XmlElement(ElementName = "CNPJ")]
        public string? CNPJ { get; set; }

        [XmlElement(ElementName = "CPF")]
        public string? CPF { get; set; }

        [XmlElement(ElementName = "idEstrangeiro")]
        public string? IdEstrangeiro { get; set; }

        [XmlElement(ElementName = "xNome")]
        public string? XNome { get; set; }        

        [XmlElement(ElementName = "IE")]
        public string? IE { get; set; }

        [XmlElement(ElementName = "enderDest")]
        public Ender Ender { get; set; }

    }

    [XmlRoot(ElementName = "det")]
    public class Det
    {

        //[XmlAttribute(AttributeName = "nItem")]
        //public int NItem { get; set; }

        [XmlElement(ElementName = "prod")]
        public Prod Prod { get; set; }

        [XmlElement(ElementName = "imposto")]
        public Imposto Imposto { get; set; }

    }


    [XmlRoot(ElementName = "Imposto")]
    public class Imposto
    {
        [XmlElement(ElementName = "vTotTrib")]
        public double VTotTrib { get; set; }

        [XmlElement(ElementName = "ICMS")]
        public ICMS ICMS { get; set; }

        [XmlElement(ElementName = "IPI")]
        public IPI IPI { get; set; }

        [XmlElement(ElementName = "PIS")]
        public PIS PIS { get; set; }

        [XmlElement(ElementName = "COFINS")]
        public COFINS COFINS { get; set; }

        [XmlElement(ElementName = "ICMSUFDest")]
        public ICMSUFDest ICMSUFDest { get; set; }
    }

    [XmlRoot(ElementName = "ICMS")]
    public class ICMS
    {
        [XmlElement(ElementName = "ICMS00")]
        public ICMSBase ICMS00 { get; set; }

        [XmlElement(ElementName = "ICMS10")]
        public ICMSBase ICMS10 { get; set; }

        [XmlElement(ElementName = "ICMS20")]
        public ICMSBase ICMS20 { get; set; }

        [XmlElement(ElementName = "ICMS30")]
        public ICMSBase ICMS30 { get; set; }

        [XmlElement(ElementName = "ICMS40")]
        public ICMSBase ICMS40 { get; set; }

        [XmlElement(ElementName = "ICMS41")]
        public ICMSBase ICMS41 { get; set; }

        [XmlElement(ElementName = "ICMS50")]
        public ICMSBase ICMS50 { get; set; }

        [XmlElement(ElementName = "ICMS51")]
        public ICMSBase ICMS51 { get; set; }

        [XmlElement(ElementName = "ICMS60")]
        public ICMSBase ICMS60 { get; set; }
        [XmlElement(ElementName = "ICMS70")]
        public ICMSBase ICMS70 { get; set; }

        [XmlElement(ElementName = "ICMS90")]
        public ICMSBase ICMS90 { get; set; }

        [XmlElement(ElementName = "ICMSSN101")]
        public ICMSBase ICMSSN101 { get; set; }

        [XmlElement(ElementName = "ICMSSN102")]
        public ICMSBase ICMSSN102 { get; set; }

        [XmlElement(ElementName = "ICMSSN103")]
        public ICMSBase ICMSSN103 { get; set; }

        [XmlElement(ElementName = "ICMSSN201")]
        public ICMSBase ICMSSN201 { get; set; }

        [XmlElement(ElementName = "ICMSSN202")]
        public ICMSBase ICMSSN202 { get; set; }

        [XmlElement(ElementName = "ICMSSN203")]
        public ICMSBase ICMSSN203 { get; set; }

        [XmlElement(ElementName = "ICMSSN400")]
        public ICMSBase ICMSSN400 { get; set; }

        [XmlElement(ElementName = "ICMSSN500")]
        public ICMSBase ICMSSN500 { get; set; }

        [XmlElement(ElementName = "ICMSSN900")]
        public ICMSBase ICMSSN900 { get; set; }  

        [XmlElement(ElementName = "ICMSST")]
        public ICMSBase ICMSST { get; set; }  

        [XmlElement(ElementName = "ICMSPART")]
        public ICMSBase ICMSPART { get; set; }        
    }

    public class ICMSBase
    {
        [XmlElement(ElementName = "orig")]
        public int Orig { get; set; }

        [XmlElement(ElementName = "CST")]
        public string CST { get; set; }

        [XmlElement(ElementName = "pRedBC")]
        public decimal PRedBC { get; set; }

        [XmlElement(ElementName = "modBC")]
        public int ModBC { get; set; }

        [XmlElement(ElementName = "vBC")]
        public string? VBC { get; set; }

        [XmlElement(ElementName = "pICMS")]
        public decimal? PICMS { get; set; }

        [XmlElement(ElementName = "vICMS")]
        public decimal? VICMS { get; set; }
        
        [XmlElement(ElementName = "modBCST")]
        public int ModBCST { get; set; }
        
        [XmlElement(ElementName = "pMVAST")]
        public decimal? PMVAST { get; set; }
        
        [XmlElement(ElementName = "pRedBCST")]
        public decimal? PRedBCST { get; set; }
        
        [XmlElement(ElementName = "vBCST")]
        public decimal? VBCST { get; set; }
        
        [XmlElement(ElementName = "pICMSST")]
        public decimal? PICMSST { get; set; }
        
        [XmlElement(ElementName = "vICMSST")]
        public decimal? VICMSST { get; set; }
        
        [XmlElement(ElementName = "UFST")]
        public string UFST { get; set; }
        
        [XmlElement(ElementName = "pBCop")]
        public decimal? PBCop { get; set; }
        
        [XmlElement(ElementName = "vBCSTRet")]
        public decimal? VBCSTRet { get; set; }
        
        [XmlElement(ElementName = "vICMSSTRet")]
        public decimal? VICMSSTRet { get; set; }
        
        [XmlElement(ElementName = "motDesICMS")]
        public int? MotDesICMS { get; set; }
        
        [XmlElement(ElementName = "vBCSTDest")]
        public decimal? VBCSTDest { get; set; }
        
        [XmlElement(ElementName = "vICMSSTDest")]
        public decimal? VICMSSTDest { get; set; }
        
        [XmlElement(ElementName = "pCredSN")]
        public decimal? PCredSN { get; set; }
        
        [XmlElement(ElementName = "vCredICMSSN")]
        public decimal? VCredICMSSN { get; set; }
        
        [XmlElement(ElementName = "vICMSDeson")]
        public decimal? VICMSDeson { get; set; }
        
        [XmlElement(ElementName = "vICMSOp")]
        public decimal? VICMSOp { get; set; }
        
        [XmlElement(ElementName = "pDif")]
        public decimal? PDif { get; set; }
        
        [XmlElement(ElementName = "vICMSDif")]
        public decimal? VICMSDif { get; set; }
        
        [XmlElement(ElementName = "vBCFCP")]
        public decimal? VBCFCP { get; set; }
        
        [XmlElement(ElementName = "pFCP")]
        public decimal? PFCP { get; set; }
        
        [XmlElement(ElementName = "vFCP")]
        public decimal? VFCP { get; set; }
        
        [XmlElement(ElementName = "vBCFCPST")]
        public decimal? VBCFCPST { get; set; }
        
        [XmlElement(ElementName = "pFCPST")]
        public decimal? PFCPST { get; set; }
        
        [XmlElement(ElementName = "vFCPST")]
        public decimal? VFCPST { get; set; }
        
        [XmlElement(ElementName = "vBCFCPSTRet")]
        public decimal? VBCFCPSTRet { get; set; }
        
        [XmlElement(ElementName = "pFCPSTRet")]
        public decimal? PFCPSTRet { get; set; }
        
        [XmlElement(ElementName = "vFCPSTRet")]
        public decimal? VFCPSTRet { get; set; }
        
        [XmlElement(ElementName = "pST")]
        public decimal? PST { get; set; }
    }


    public class IPI
    {
        [XmlElement(ElementName = "cEnq")]
        public int CEnq { get; set; }

        [XmlElement(ElementName = "IPINT")]
        public IPINT IPINT { get; set; }

        [XmlElement(ElementName = "IPITrib")]
        public IPITrib IPITrib { get; set; }
    }

    public class IPINT
    {
        [XmlElement(ElementName = "CST")]
        public string CST { get; set; }
    }

    public class IPITrib
    {
        [XmlElement(ElementName = "CST")]
        public string CST { get; set; }

        [XmlElement(ElementName = "vBC")]
        public decimal VBC { get; set; }

        [XmlElement(ElementName = "pIPI")]
        public decimal PIPI { get; set; }

        [XmlElement(ElementName = "vIPI")]
        public decimal VIPI { get; set; }
    }

    public class PIS
    {
        [XmlElement(ElementName = "PISAliq")]
        public PISAliq PISAliq { get; set; }

        [XmlElement(ElementName = "PISOutr")]
        public PISOutr PISOutr { get; set; }
    }

    public class PISAliq
    {
        [XmlElement(ElementName = "CST")]
        public string CST { get; set; }

        [XmlElement(ElementName = "vBC")]
        public decimal VBC { get; set; }

        [XmlElement(ElementName = "pPIS")]
        public decimal PPIS { get; set; }

        [XmlElement(ElementName = "vPIS")]
        public decimal VPIS { get; set; }
    }

    public class PISOutr
    {
        [XmlElement(ElementName = "CST")]
        public string CST { get; set; }

        [XmlElement(ElementName = "vBC")]
        public decimal VBC { get; set; }

        [XmlElement(ElementName = "pPIS")]
        public decimal PPIS { get; set; }

        [XmlElement(ElementName = "vPIS")]
        public decimal VPIS { get; set; }
    }

    public class COFINS
    {
        [XmlElement(ElementName = "COFINSAliq")]
        public COFINSAliq COFINSAliq { get; set; }

        [XmlElement(ElementName = "COFINSOutr")]
        public COFINSOutr COFINSOutr { get; set; }

    }

    public class COFINSAliq
    {
        [XmlElement(ElementName = "CST")]
        public string CST { get; set; }

        [XmlElement(ElementName = "vBC")]
        public decimal VBC { get; set; }

        [XmlElement(ElementName = "pCOFINS")]
        public decimal PCOFINS { get; set; }

        [XmlElement(ElementName = "vCOFINS")]
        public decimal VCOFINS { get; set; }
    }

    public class COFINSOutr
    {
        [XmlElement(ElementName = "CST")]
        public string CST { get; set; }

        [XmlElement(ElementName = "vBC")]
        public decimal VBC { get; set; }

        [XmlElement(ElementName = "pCOFINS")]
        public decimal PCOFINS { get; set; }

        [XmlElement(ElementName = "vCOFINS")]
        public decimal VCOFINS { get; set; }
    }

    public class ICMSUFDest
    {
        [XmlElement(ElementName = "vBCUFDest")]
        public decimal VBCUFDest { get; set; }

        [XmlElement(ElementName = "vBCFCPUFDest")]
        public decimal VBCFCPUFDest { get; set; }

        [XmlElement(ElementName = "pFCPUFDest")]
        public decimal PFCPUFDest { get; set; }

        [XmlElement(ElementName = "pICMSUFDest")]
        public decimal PICMSUFDest { get; set; }

        [XmlElement(ElementName = "pICMSInter")]
        public decimal PICMSInter { get; set; }

        [XmlElement(ElementName = "pICMSInterPart")]
        public decimal PICMSInterPart { get; set; }

        [XmlElement(ElementName = "vFCPUFDest")]
        public decimal VFCPUFDest { get; set; }

        [XmlElement(ElementName = "vICMSUFDest")]
        public decimal VICMSUFDest { get; set; }

        [XmlElement(ElementName = "vICMSUFRemet")]
        public decimal VICMSUFRemet { get; set; }
    }


    [XmlRoot(ElementName = "prod")]
    public class Prod
    {

        [XmlElement(ElementName = "cProd")]
        public string? CProd { get; set; }

        [XmlElement(ElementName = "cEAN")]
        public string? CEAN { get; set; }

        [XmlElement(ElementName = "CFOP")]
        public string? CFOP { get; set; }

        [XmlElement(ElementName = "NCM")]
        public string? NCM { get; set; }

        [XmlElement(ElementName = "CEST")]
        public string? CEST { get; set; }

        [XmlElement(ElementName = "xProd")]
        public string? XProd { get; set; }

        [XmlElement(ElementName = "qCom")]
        public double QCom { get; set; }

        [XmlElement(ElementName = "uTrib")]
        public string UTRIB { get; set; }

        [XmlElement(ElementName = "uCOM")]
        public string UCOM { get; set; }

        [XmlElement(ElementName = "vUnCom")]
        public double VUnCom { get; set; }

    }

    [XmlRoot(ElementName = "infNFe")]
    public class InfNFe
    {
        [XmlElement(ElementName = "ide")]
        public Ide Ide { get; set; }

        [XmlElement(ElementName = "emit")]
        public Emit Emit { get; set; }

        [XmlElement(ElementName = "dest")]
        public Dest Dest { get; set; }

        [XmlElement(ElementName = "det")]
        public List<Det> Det { get; set; }

        [XmlElement(ElementName = "infAdic")]
        public InfAdic InfAdic { get; set; }

        [XmlElement(ElementName = "total")]
        public Total Total { get; set; }
    }

    public class Ender
    {
        [XmlElement(ElementName = "xBairro")]
        public string? XBairro { get; set; }

        [XmlElement(ElementName = "xLgr")]
        public string? XLgr { get; set; }

        [XmlElement(ElementName = "xCpl")]
        public string? XCpl { get; set; }

        [XmlElement(ElementName = "nro")]
        public string? Nro { get; set; }

        [XmlElement(ElementName = "xMun")]
        public string? XMun { get; set; }

        [XmlElement(ElementName = "UF")]
        public string? UF { get; set; }

        [XmlElement(ElementName = "CEP")]
        public string? CEP { get; set; }

        [XmlElement(ElementName = "xPais")]
        public string? xPais { get; set; }
    }

    [XmlRoot(ElementName = "NFe")]
    public class NFe
    {

        [XmlElement(ElementName = "infNFe")]
        public InfNFe InfNFe { get; set; }

    }

    [XmlRoot(ElementName = "infProt")]
    public class InfProt
    {

        [XmlElement(ElementName = "chNFe")]
        public string? ChNFe { get; set; }

        [XmlElement(ElementName = "nProt")]
        public string? NProt { get; set; }

    }

    [XmlRoot(ElementName = "infAdic")]
    public class InfAdic
    {

        [XmlElement(ElementName = "infCpl")]
        public string? InfCpl { get; set; }

        [XmlElement(ElementName = "infAdFisco")]
        public string? InfAdFisco { get; set; }

    }

    [XmlRoot(ElementName = "total")]
    public class Total
    {

        [XmlElement(ElementName = "ICMSTot")]
        public ICMSTot ICMSTot { get; set; }

    }

    [XmlRoot(ElementName = "ICMSTot")]
    public class ICMSTot
    {       
        [XmlElement(ElementName = "vNF")]
        public decimal? vNF { get; set; }

    }

    [XmlRoot(ElementName = "protNFe")]
    public class ProtNFe
    {

        [XmlElement(ElementName = "infProt")]
        public InfProt InfProt { get; set; }

    }

    [XmlRoot(ElementName = "nfeProc", Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public class NfeProc
    {
        [XmlElement(ElementName = "NFe")]
        public NFe NFe { get; set; }

        [XmlElement(ElementName = "protNFe")]
        public ProtNFe ProtNFe { get; set; }

    }


}
