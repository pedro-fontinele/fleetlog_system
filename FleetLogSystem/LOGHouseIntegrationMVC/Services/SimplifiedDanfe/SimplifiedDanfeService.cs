using LOGHouseSystem.Adapters.Extensions.NFeExtension;
using LOGHouseSystem.Models;
using LOGHouseSystem.Services.Interfaces;

namespace LOGHouseSystem.Services
{
    public class SimplifiedDanfeService : ISimplifiedDanfeService
    {
        private INFeExtension _nFeExtension;

        public SimplifiedDanfeService(INFeExtension nFeExtension)
        {
            _nFeExtension = nFeExtension;
        }

        public async Task<string> GenerateSimplifiedDanfeByReceiptNote(string danfeBaseFileContent, Invoice receiptNote)
        {
            var xmlPath = $"{Environment.XmlUploadPath}/{receiptNote.AccessKey}.xml";

            NfeProc nfe = await _nFeExtension.GetNfeByPathAsync(xmlPath);

            if (nfe == null)
            {
                return null;
            }

            var danfe = danfeBaseFileContent;

            danfe = danfe.Replace("%nro_nota%", nfe.NFe.InfNFe.Ide.NNF.ToString());
            danfe = danfe.Replace("%serie%", nfe.NFe.InfNFe.Ide.Serie.ToString());
            danfe = danfe.Replace("%datahora_emissao%", Convert.ToDateTime(nfe.NFe.InfNFe.Ide.DhEmi).ToString("dd/MM/yyyy h:mm:ss"));
            danfe = danfe.Replace("%chave_de_acesso_nfe%", nfe.ProtNFe.InfProt.ChNFe.ToString());
            danfe = danfe.Replace("%endereco_completo%", $"{nfe.NFe.InfNFe.Emit.Ender.CEP} {nfe.NFe.InfNFe.Emit.Ender.XLgr}, {nfe.NFe.InfNFe.Emit.Ender.Nro} - {nfe.NFe.InfNFe.Emit.Ender.XBairro} - {nfe.NFe.InfNFe.Emit.Ender.XMun} - {nfe.NFe.InfNFe.Emit.Ender.UF} - {nfe.NFe.InfNFe.Emit.Ender.xPais}");
            danfe = danfe.Replace("%numero_documento%", $"{nfe.NFe.InfNFe.Emit.CNPJ}");
            danfe = danfe.Replace("%nome_emitente%", $"{nfe.NFe.InfNFe.Emit.XNome}");
            danfe = danfe.Replace("%inscricao_estadual%", $"{nfe.NFe.InfNFe.Emit.IE}");
            danfe = danfe.Replace("%nro_protocolo%", $"{nfe.ProtNFe.InfProt.NProt}");

            danfe = danfe.Replace("%nome_destinatario%", $"{nfe.NFe.InfNFe.Dest.XNome}");
            danfe = danfe.Replace("%endereco_destinatario%", $"{nfe.NFe.InfNFe.Dest.Ender.XLgr}, {nfe.NFe.InfNFe.Dest.Ender.Nro}");
            danfe = danfe.Replace("%cep%", $"{nfe.NFe.InfNFe.Dest.Ender.CEP}");
            danfe = danfe.Replace("%bairro%", $"{nfe.NFe.InfNFe.Dest.Ender.XBairro}");
            danfe = danfe.Replace("%uf%", $"{nfe.NFe.InfNFe.Dest.Ender.UF}");
            danfe = danfe.Replace("%cidade%", $"{nfe.NFe.InfNFe.Dest.Ender.XMun}");


            // Writing informacoes_adicionais
            int maxInfAd = 10;
            bool[] indAdWrited = new bool[] { false, false, false, false, false, false, false, false, false, false };
            bool thereArePendingInfAd = true;
            int lasInfAd = 0;
            int maxLenthLine = 50;


            if (nfe.NFe.InfNFe?.InfAdic?.InfCpl != null)
            {                
                while (lasInfAd < maxInfAd)
                {
                    lasInfAd++;

                    indAdWrited[lasInfAd - 1] = true;

                    if (nfe.NFe.InfNFe.InfAdic.InfCpl.Length > lasInfAd * maxLenthLine)
                    {
                        danfe = danfe.Replace($"%dados_adicionais_{lasInfAd}%", nfe.NFe.InfNFe.InfAdic.InfCpl.Substring((lasInfAd - 1) * maxLenthLine, maxLenthLine));
                    }
                    else
                    {
                        danfe = danfe.Replace($"%dados_adicionais_{lasInfAd}%", nfe.NFe.InfNFe.InfAdic.InfCpl.Substring((lasInfAd - 1) * maxLenthLine, nfe.NFe.InfNFe.InfAdic.InfCpl.Length - (lasInfAd - 1) * maxLenthLine));
                        break;
                    }
                }               
            }

            // Cleaning informacoes_adicionais that weren't used
            for (int i = 0; i < maxInfAd; i++)
            {
                if (!indAdWrited[i])
                {
                    danfe = danfe.Replace($"%dados_adicionais_{i + 1}%", "");
                }
            }

            return danfe;
        }

        public string GenerateSimplifiedDanfe(string danfeBaseFileContent, ExpeditionOrder expeditionOrder, Client client)
        {
            var danfe = danfeBaseFileContent;

            danfe = danfe.Replace("%nro_nota%", expeditionOrder.InvoiceNumber.ToString());
            danfe = danfe.Replace("%serie%", expeditionOrder.InvoiceSerie.ToString());
            danfe = danfe.Replace("%datahora_emissao%", expeditionOrder.IssueDate != null ? Convert.ToDateTime(expeditionOrder.IssueDate).ToString("dd/MM/yyyy h:mm:ss") : "");
            danfe = danfe.Replace("%chave_de_acesso_nfe%", expeditionOrder.InvoiceAccessKey);
            danfe = danfe.Replace("%endereco_completo%", $"{client.Adress}");
            danfe = danfe.Replace("%numero_documento%", $"{client.Cnpj}");
            danfe = danfe.Replace("%nome_emitente%", client.SocialReason);
            danfe = danfe.Replace("%inscricao_estadual%", $"");
            danfe = danfe.Replace("%nro_protocolo%", $"");

            if (expeditionOrder.ShippingDetails != null)
            {
                danfe = danfe.Replace("%nome_destinatario%", $"{expeditionOrder.ShippingDetails.Name}");
                danfe = danfe.Replace("%endereco_destinatario%", $"{expeditionOrder.ShippingDetails.Address}, {expeditionOrder.ShippingDetails.Number}");
                danfe = danfe.Replace("%cep%", $"{expeditionOrder.ShippingDetails.Cep}");
                danfe = danfe.Replace("%bairro%", $"{expeditionOrder.ShippingDetails.Neighborhood}");
                danfe = danfe.Replace("%uf%", $"{expeditionOrder.ShippingDetails.Uf}");
                danfe = danfe.Replace("%cidade%", $"{expeditionOrder.ShippingDetails.City}");
            }


            danfe = danfe.Replace("%dados_adicionais_1%", $"");
            danfe = danfe.Replace("%dados_adicionais_2%", $"");
            danfe = danfe.Replace("%dados_adicionais_3%", $"");
            danfe = danfe.Replace("%dados_adicionais_4%", $"");
            danfe = danfe.Replace("%dados_adicionais_5%", $"");
            danfe = danfe.Replace("%dados_adicionais_6%", $"");
            danfe = danfe.Replace("%dados_adicionais_7%", $"");
            danfe = danfe.Replace("%dados_adicionais_8%", $"");
            danfe = danfe.Replace("%dados_adicionais_9%", $"");
            danfe = danfe.Replace("%dados_adicionais_10%", $"");

            return danfe;
        }
    }
}
