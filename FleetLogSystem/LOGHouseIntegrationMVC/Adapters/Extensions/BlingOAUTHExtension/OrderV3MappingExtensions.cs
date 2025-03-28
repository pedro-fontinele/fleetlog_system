using LOGHouseSystem.Adapters.Extensions.BlingExtension.Dto.Hook.Request;
using LOGHouseSystem.Adapters.Extensions.BlingOAUTHExtension.Dto;

namespace LOGHouseSystem.Adapters.Extensions.BlingOAUTHExtension
{
    public static class OrderV3MappingExtensions
    {
        public static BlingPedidoCallbackRequest MapOrderBlingV3ToV2(OrderBlingV3 orderBlingV3)
        {
            var blingPedido = new BlingPedidoCallbackRequest
            {
                Desconto = orderBlingV3.desconto?.valor.ToString(),
                Observacoes = orderBlingV3.observacoes,
                Observacaointerna = orderBlingV3.observacoesInternas,
                Data = orderBlingV3.data,
                Numero = orderBlingV3.numero.ToString(),
                NumeroOrdemCompra = orderBlingV3.numeroPedidoCompra,
                Vendedor = orderBlingV3.vendedor?.id.ToString(),
                Valorfrete = orderBlingV3.transporte?.frete.ToString(),
                Outrasdespesas = orderBlingV3.outrasDespesas.ToString(),
                Totalprodutos = orderBlingV3.itens.Sum(e => e.valor).ToString(),
                Totalvenda = orderBlingV3.total.ToString(),
                Situacao = orderBlingV3.situacao?.valor.ToString(),
                Loja = orderBlingV3.loja?.id.ToString(),
                NumeroPedidoLoja = orderBlingV3.numeroLoja,
                TipoIntegracao = "manual", // Você pode ajustar conforme necessário
                Nota = MapNotaBlingV3ToV2(orderBlingV3),
                Intermediador = MapIntermediadorBlingV3ToV2(orderBlingV3.intermediador),
                Itens = orderBlingV3.itens?.Select(MapItemBlingV3ToV2).ToList(),
                Transporte = MapTransporteBlingV3ToV2(orderBlingV3.transporte),
                Client = MapClientBlingV3ToV2(orderBlingV3.contato)
            };

            return blingPedido;
        }

        private static BlingNotaCallbackRequest MapNotaBlingV3ToV2(OrderBlingV3 orderBlingV3)
        {
            if (orderBlingV3 != null && orderBlingV3.situacao?.valor.ToString() == "7") // Adapte conforme necessário
            {
                return new BlingNotaCallbackRequest
                {
                    Serie = "Série da Nota",
                    Numero = "Número da Nota",
                    DataEmissao = DateTime.Now, // Ajuste conforme necessário
                    Situacao = "Situação da Nota", // Ajuste conforme necessário
                    ValorNota = 0.0f, // Ajuste conforme necessário
                    ChaveAcesso = "Chave de Acesso da Nota" // Ajuste conforme necessário
                };
            }

            return null;
        }

        private static BlingIntermediadorCallbackRequest MapIntermediadorBlingV3ToV2(IntermediadorBlingV3 intermediadorBlingV3)
        {
            return intermediadorBlingV3 != null
                ? new BlingIntermediadorCallbackRequest
                {
                    Cnpj = intermediadorBlingV3.cnpj,
                    NomeUsuario = intermediadorBlingV3.nomeUsuario
                }
                : null;
        }

        private static BlingItenCallbackRequest MapItemBlingV3ToV2(ItenBlingV3 itenBlingV3)
        {
            return itenBlingV3 != null
                ? new BlingItenCallbackRequest
                {
                    Item = new BlingItemCallbackRequest
                    {
                        Codigo = itenBlingV3.codigo,
                        Descricao = itenBlingV3.descricao,
                        Quantidade = itenBlingV3.quantidade,
                        Valorunidade = Convert.ToDecimal(itenBlingV3.valor),
                        // Mapeie os demais campos conforme necessário
                    }
                }
                : null;
        }

        private static BlingTransporteCallbackRequest MapTransporteBlingV3ToV2(TransporteBlingV3 transporteBlingV3)
        {
            var item = transporteBlingV3.volumes.Select(e => MapVolumeBlingV3ToV2(e)).ToList();

            return transporteBlingV3 != null
                ? new BlingTransporteCallbackRequest
                {
                    EnderecoEntrega = MapEnderecoEntregaBlingV3ToV2(transporteBlingV3.contato),
                    Volumes = transporteBlingV3.volumes?.Select(e => new BlingVolumesCallbackRequest() { Volume = MapVolumeBlingV3ToV2(e) }).ToList()
                    
                }
                : null;
        }

        private static BlingEnderecoEntregaCallbackRequest MapEnderecoEntregaBlingV3ToV2(ContatoBlingV3 contatoBlingV3)
        {
            return contatoBlingV3 != null
                ? new BlingEnderecoEntregaCallbackRequest
                {
                    Nome = contatoBlingV3.nome,
                    Endereco = null, // contatoBlingV3.endereco,
                    Numero = null, //contatoBlingV3.numero,
                    Complemento = null, //contatoBlingV3.complemento,
                    Cidade = null, //contatoBlingV3.municipio,
                    Bairro = null, //contatoBlingV3.bairro,
                    Cep = null, //contatoBlingV3.cep,
                    Uf = null, //contatoBlingV3.uf
                }
                : null;
        }

        private static BlingVolumeCallbackRequest MapVolumeBlingV3ToV2(VolumeBlingV3 volumeBlingV3)
        {
            return volumeBlingV3 != null
                ? new BlingVolumeCallbackRequest
                {
                    CodigoRastreamento = volumeBlingV3.codigoRastreamento,
                    Servico = volumeBlingV3.servico
                }
                : null;
        }

        private static BlingClientCallbackRequest MapClientBlingV3ToV2(ContatoBlingV3 contatoBlingV3)
        {
            return contatoBlingV3 != null
                ? new BlingClientCallbackRequest
                {
                    Nome = contatoBlingV3.nome,
                    Cnpj = contatoBlingV3.numeroDocumento,
                    // Mapeie os demais campos conforme necessário
                }
                : null;
        }
    }
}
