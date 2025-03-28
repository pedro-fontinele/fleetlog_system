using LOGHouseSystem.Adapters.Websocket;
using LOGHouseSystem.Controllers.API.BarcodeColectorApi.Request;
using LOGHouseSystem.Controllers.API.BarcodeColectorApi.Response;
using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Helpers;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LOGHouseSystem.Controllers.API.BarcodeColectorApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class BarcodeColectorApiController : ControllerBase
    {
        private ReceiptNoteItemRepository _receiptNoteItemRepository;
        private ReceiptNoteRepository _receiptNoteRepository;
        private PickingListRepository _pickingListRepository;
        private IReceiptNoteService _receiptNoteService;
        private ICaixaMastersRepository _caixaMastersRepository;
        private IProductRepository _productRepository;
        private IPositionAndProductRepository _positionAndProductRepository;
        private IAddressingPositionRepository _addressingPositionRepository;
        private IReceiptNoteItemService _receiptNoteItemService;
        private IPickingListItemService _pickingListItemService;
        private IPackingService _packingService;
        private IPackingRepository _packingRepository;
        private IPackingItemService _packingItemService;
        private ITransportationPersonService _transportationPersonService;
        private IShippingCompanyService _shippingCompanyService;
        private IPackingListTransportationService _packingListTransportationService;
        private IPackingListTransportationRepository _packingListTransportationRepository;
        private IExpeditionOrderRepository _expeditionOrderRepository;
        private ITransportationPersonRepository _transportationPersonRepository;
        private IShippingCompanyRepository _shippingCompanyRepository;
        private readonly IPickingListHistoryService _pickingListHistoryService;
        private readonly IConfiguration _configuration;
        private readonly IExpeditionOrderHistoryService _expeditionOrderHistoryService;

        public BarcodeColectorApiController(AppDbContext context,
            ICaixaMastersRepository caixaMastersRepository,
            IProductRepository productRepository,
            IPositionAndProductRepository positionAndProductRepository,
            IAddressingPositionRepository addressingPositionRepository,
            IReceiptNoteItemService receiptNoteItemService,
            IPackingService packingService,
            IPickingListItemService pickingListItemService,
            IPackingRepository packingRepository,
            IPackingItemService packingItemService,
            ITransportationPersonService transportationPersonService,
            IShippingCompanyService shippingCompanyService,
            IPackingListTransportationService packingListTransportationService,
            IPackingListTransportationRepository packingListTransportationRepository,
            IExpeditionOrderRepository expeditionOrderRepository,
            ITransportationPersonRepository transportationPersonRepository,
            IShippingCompanyRepository shippingCompanyRepository,
            IPickingListHistoryService pickingListHistoryService,
            IConfiguration configuration,
            IReceiptNoteService receiptNoteService,
            IExpeditionOrderHistoryService expeditionOrderHistoryService)
        {
            _receiptNoteItemRepository = new ReceiptNoteItemRepository();
            _receiptNoteRepository = new ReceiptNoteRepository();
            _pickingListRepository = new PickingListRepository();
            _receiptNoteService = receiptNoteService;
            _caixaMastersRepository = caixaMastersRepository;
            _productRepository = productRepository;
            _positionAndProductRepository = positionAndProductRepository;
            _addressingPositionRepository = addressingPositionRepository;
            _receiptNoteItemService = receiptNoteItemService;
            _pickingListItemService = pickingListItemService;
            _packingRepository = packingRepository;
            _packingService = packingService;
            _packingItemService = packingItemService;
            _transportationPersonService = transportationPersonService;
            _shippingCompanyService = shippingCompanyService;
            _packingListTransportationService = packingListTransportationService;
            _packingListTransportationRepository = packingListTransportationRepository;
            _expeditionOrderRepository = expeditionOrderRepository;
            _transportationPersonRepository = transportationPersonRepository;
            _shippingCompanyRepository = shippingCompanyRepository;
            _pickingListHistoryService = pickingListHistoryService;
            _configuration = configuration;
            _expeditionOrderHistoryService = expeditionOrderHistoryService;
        }

        [Route("Health")]
        [HttpGet]
        public ActionResult Health()
        {
            try
            {
                return Ok("Working...");
            }
            catch (Exception ex)
            {
                return Problem((ex.InnerException ?? ex).Message);
            }
        }

        [Route("GetNotesByStatus/{statusId}")]
        [HttpGet]
        [Authorize(Roles = "Admin,Gerente,Funcionário")]
        public ActionResult<List<GetNotesByStatusResponse>> GetNotesByStatus(NoteStatus statusId)
        {
            try
            {
                List<ReceiptNote> notes = _receiptNoteRepository.GetByStatus(statusId);

                List<GetNotesByStatusResponse> responseNotes = notes.Select(note => new GetNotesByStatusResponse
                {
                    Client = new NoteCustomer
                    {
                        Cnpj = note.Client.Cnpj,
                        SocialReason = note.Client.SocialReason
                    },
                    ClientId = note.ClientId,
                    EntryDate = note.EntryDate,
                    Id = note.Id,
                    Number = note.Number,
                    Status = note.Status,
                    ValidationPercent = _receiptNoteService.getPercent(note)
                }).ToList();

                return Ok(responseNotes);
            }
            catch (Exception ex)
            {
                return Problem((ex.InnerException ?? ex).Message);
            }
        }

        //[Route("GetNotesByFilter")]
        //[HttpPost]
        //[Authorize(Roles = "Admin,Gerente,Funcionário")]
        //public ActionResult GetNotesByFilter([FromBody] BarcodeColectorFilter filter)
        //{
        //    try
        //    {
        //        return Ok(_receiptNoteRepository.GetByFilter(filter));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Problem((ex.InnerException ?? ex).Message);
        //    }
        //}

        [Route("GetNoteById/{id}")]
        [HttpGet]
        [Authorize(Roles = "Admin,Gerente,Funcionário")]
        public ActionResult<ReceiptNote> GetNoteById(int id)
        {
            try
            {
                var receiptNote = _receiptNoteRepository.GetById(id);
                receiptNote.ReceiptNoteItems = receiptNote.ReceiptNoteItems.Select(rni => new ReceiptNoteItem
                {
                    Code = rni.Code,
                    Description = rni.Description,
                    Ean = rni.Ean,
                    Id = rni.Id,
                    ItemStatus = rni.ItemStatus,
                    Quantity = rni.Quantity,
                    QuantityInspection = rni.QuantityInspection,
                    ReceiptNote = null,
                    ReceiptNoteId = rni.ReceiptNoteId,
                    Value = rni.Value
                }).ToList();

                return Ok(receiptNote);
            }
            catch (Exception ex)
            {
                return Problem((ex.InnerException ?? ex).Message);
            }
        }

        [Route("ValidateNoteItemBox")]
        [HttpPost]
        [Authorize(Roles = "Admin,Gerente,Funcionário")]
        public ActionResult ValidateNoteItemBox([FromBody] ValidateNoteItemBoxRequest data)
        {
            try
            {
                CaixaMaster? caixaMaster = _caixaMastersRepository.GetByCode(data.BoxCode);

                if (caixaMaster == null) return Problem("Código de Caixa Master não encontrada");

                var noteItem = _receiptNoteItemRepository.GetById(caixaMaster.ReceiptNoteItemId);
                if (noteItem == null) throw new Exception("Item não encontrado");

                if (noteItem.QuantityInspection + caixaMaster.Quantity > noteItem.Quantity) throw new Exception("Quantidade da caixssa ultrapassa quantidade de produtos da nota");

                noteItem.QuantityInspection += caixaMaster.Quantity;

                if (noteItem.QuantityInspection < noteItem.Quantity)
                    noteItem.ItemStatus = NoteItemStatus.EmAndamento;

                if (noteItem.QuantityInspection == noteItem.Quantity)
                    noteItem.ItemStatus = NoteItemStatus.ItemOk;

                noteItem = _receiptNoteItemRepository.Update(noteItem);

                var note = _receiptNoteRepository.GetById(data.ReceiptNoteId);

                if (note == null) throw new Exception("Item não encontrado");

                if (note.Status == NoteStatus.Aguardando)
                {
                    note.Status = NoteStatus.EmAndamento;

                    note = _receiptNoteRepository.Update(note);
                }

                try
                {
                    var noteDashboarVM = _receiptNoteService.ReceiptNoteToDashboardMapper(note);

                    JObject message = JObject.FromObject(noteDashboarVM);

                    WebsocketClient websocketClient = new WebsocketClient();
                    websocketClient.SendMessage($"{message}");
                }
                catch (Exception ex2)
                {
                }

                return Ok(noteItem);
            }
            catch (Exception ex)
            {
                return Problem((ex.InnerException ?? ex).Message);
            }
        }

        [Route("ValidateNoteItem")]
        [HttpPost]
        [Authorize(Roles = "Admin,Gerente,Funcionário")]
        public ActionResult ValidateNoteItem([FromBody] ValidateNoteItemRequest data)
        {
            try
            {
                var noteItem = _receiptNoteItemService.SearchItemToValidate(data.ReceiptNoteId, data.Ean);

                if (noteItem.QuantityInspection + 1 > noteItem.Quantity) throw new Exception("Produto já atingiu quantidade da nota");

                noteItem.QuantityInspection++;

                if (noteItem.QuantityInspection < noteItem.Quantity)
                    noteItem.ItemStatus = NoteItemStatus.EmAndamento;

                if (noteItem.QuantityInspection == noteItem.Quantity)
                    noteItem.ItemStatus = NoteItemStatus.ItemOk;

                noteItem = _receiptNoteItemRepository.Update(noteItem);

                var note = _receiptNoteRepository.GetById(data.ReceiptNoteId);

                if (note == null) throw new Exception("Item não encontrado");

                if (note.Status == NoteStatus.Aguardando)
                {
                    note.Status = NoteStatus.EmAndamento;
                    note = _receiptNoteRepository.Update(note);
                }

                try
                {
                    var noteDashboarVM = _receiptNoteService.ReceiptNoteToDashboardMapper(note);

                    JObject message = JObject.FromObject(noteDashboarVM);

                    WebsocketClient websocketClient = new WebsocketClient();
                    websocketClient.SendMessage($"{message}");
                }
                catch (Exception ex2)
                {
                }

                return Ok(noteItem);
            }
            catch (Exception ex)
            {
                return Problem((ex.InnerException ?? ex).Message);
            }
        }

        [Route("ValidateNoteItemByList")]
        [HttpPost]
        [Authorize(Roles = "Admin,Gerente,Funcionário")]
        public ActionResult ValidateNoteItemByList([FromBody] ValidateNoteItemRequest[] data)
        {
            List<ValidateNoteItemRequest> itemsWithErrors = new List<ValidateNoteItemRequest>();
            int successCounter = 0;
            foreach(var item in data)
            {
                try
                {
                    var noteItem = _receiptNoteItemService.SearchItemToValidate(item.ReceiptNoteId, item.Ean);

                    if (noteItem.QuantityInspection + 1 > noteItem.Quantity) throw new Exception("Produto já atingiu quantidade da nota");

                    noteItem.QuantityInspection++;

                    if (noteItem.QuantityInspection < noteItem.Quantity)
                        noteItem.ItemStatus = NoteItemStatus.EmAndamento;

                    if (noteItem.QuantityInspection == noteItem.Quantity)
                        noteItem.ItemStatus = NoteItemStatus.ItemOk;

                    noteItem = _receiptNoteItemRepository.Update(noteItem);

                    var note = _receiptNoteRepository.GetById(item.ReceiptNoteId);

                    if (note == null) throw new Exception("Item não encontrado");

                    if (note.Status == NoteStatus.Aguardando)
                    {
                        note.Status = NoteStatus.EmAndamento;
                        note = _receiptNoteRepository.Update(note);
                    }

                    try
                    {
                        var noteDashboarVM = _receiptNoteService.ReceiptNoteToDashboardMapper(note);

                        JObject message = JObject.FromObject(noteDashboarVM);

                        WebsocketClient websocketClient = new WebsocketClient();
                        websocketClient.SendMessage($"{message}");
                    }
                    catch (Exception ex2)
                    {
                    }

                    successCounter++;
                }
                catch
                {
                    itemsWithErrors.Add(item);
                }
            }

            return Ok(new ValidateNoteItemByListResponse
            {
                Sent = data.Length,
                Success= successCounter,
                Error = itemsWithErrors
            });
        }

        [Route("ConfirmNoteValidation/{id}")]
        [HttpPost]
        [Authorize(Roles = "Admin,Gerente,Funcionário")]
        public ActionResult ConfirmNoteValidation(int id)
        {
            try
            {
                ResponseDTO response = _receiptNoteService.ConfirmReceiptNote(id);
                if (!response.Success) throw new Exception(response.Message);
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem((ex.InnerException ?? ex).Message);
            }
        }

        [Route("GetProductsToAddressing/{id}")]
        [HttpGet]
        [Authorize(Roles = "Admin,Gerente,Funcionário")]
        public ActionResult<GetProductsToAddressingResponse> GetProductsToAddressing(int id)
        {
            try
            {
                ReceiptNote receiptNote = _receiptNoteRepository.GetById(id);
                if (receiptNote.Status != NoteStatus.AguardandoEnderecamento) throw new Exception("Este pedido não está aguardando endereçamento");

                List<ReceiptNoteItem> items = receiptNote.ReceiptNoteItems;

                List<ProductAddressing> productsResponseList = new List<ProductAddressing>();
                foreach (ReceiptNoteItem item in items)
                {
                    Product product = _productRepository.GetByEan($"{item.Ean}", receiptNote.ClientId);
                    ProductAddressing productAddressing = new ProductAddressing
                    {
                        ProductId = product.Id,
                        ReceiptNoteItemID = item.Id,
                        Code = product.Code,
                        Description = product.Description,
                        Ean = product.Ean,
                        StockQuantity = product.StockQuantity,
                        Status = item.ItemStatus
                    };

                    productsResponseList.Add(productAddressing);
                }

                return Ok(new GetProductsToAddressingResponse
                {
                    ReceiptNoteID = id,
                    Products = productsResponseList
                });

            }
            catch (Exception ex)
            {
                return Problem((ex.InnerException ?? ex).Message);
            }
        }

        [Route("SetProductAddress")]
        [HttpPost]
        [Authorize(Roles = "Admin,Gerente,Funcionário")]
        public ActionResult SetProductAddress([FromBody] SetProductAddressRequest data)
        {
            try
            {
                var noteItem = _receiptNoteItemRepository.GetById(data.ReceiptNoteItemID);
                if (noteItem == null) throw new Exception("Item não encontrado");

                if (noteItem.ItemStatus == NoteItemStatus.Finalizado)
                    throw new Exception("Item já foi endereçado");

                if (noteItem.ItemStatus != NoteItemStatus.AguardandoEnderecamento)
                    throw new Exception("Item não pode ser endereçado");

                var product = _productRepository.GetById(data.ProductId);
                if (product == null) throw new Exception("Produto não encontrado");

                List<AddressingPosition> position = _addressingPositionRepository.SearchByName($"{data.AddressingPositionName}");
                if (position.Count == 0) throw new Exception("Posição não encontrada");
                if (position.Count > 1) throw new Exception("Mais de uma posição encontrada, por favor escreva o nome completo da posição");

                if (_positionAndProductRepository.ProductAlreadyAssociated(position[0].AddressingPositionID, product.Id)) Ok(noteItem);

                _positionAndProductRepository.Add(new PositionAndProduct
                {
                    AddressingPositionId = position[0].AddressingPositionID,
                    ProductId = product.Id
                });
                noteItem.ItemStatus = NoteItemStatus.Finalizado;
                noteItem = _receiptNoteItemRepository.Update(noteItem);

                ReceiptNote receiptNote = _receiptNoteRepository.GetById(noteItem.ReceiptNoteId);
                if (receiptNote.ReceiptNoteItems.Where(item => item.ItemStatus != NoteItemStatus.Finalizado).Count() == 0)
                    receiptNote.Status = NoteStatus.Finalizada;

                _receiptNoteRepository.Update(receiptNote);

                return Ok(noteItem);
            }
            catch (Exception ex)
            {
                return Problem((ex.InnerException ?? ex).Message);
            }
        }

        [Route("GetPickingListByStatus/{statusId}")]
        [HttpGet]
        [Authorize(Roles = "Admin,Gerente,Funcionário")]
        public ActionResult<List<GetPickingListByStatusResponse>> GetPickingListByStatus(PickingListStatus statusId)
        {
            try
            {
                List<PickingList> pickingList = _pickingListRepository.GetByStatus(statusId);

                List<GetPickingListByStatusResponse> responseNotes = pickingList.Select(picking => new GetPickingListByStatusResponse
                {
                    Client = new NoteCustomer
                    {
                        //Cnpj = picking.Client.Cnpj,
                        //SocialReason = picking.Client.SocialReason
                    },
                    Id = picking.Id,
                    Description = picking.Description,
                    Quantity = picking.Quantity,
                    Responsible = picking.Responsible,
                    Status = picking.Status,
                    CreatedAt = picking.CreatedAt,
                    Priority = picking.Priority,
                    PriorityColor = ColorHelper.PriorityColor(picking.Priority),
                    MarketPlace = picking.MarketPlace?.GetDescription() ?? ""
                }).ToList();

                return Ok(responseNotes);
            }
            catch (Exception ex)
            {
                return Problem((ex.InnerException ?? ex).Message);
            }
        }

        [Route("GetPackingByStatus/{statusId}")]
        [HttpGet]
        [Authorize(Roles = "Admin,Gerente,Funcionário")]
        public async Task<ActionResult<List<GetPickingListByStatusResponse>>> GetPackingByStatus(PackingStatus statusId)
        {
            try
            {
                List<Packing> packingList = await _packingRepository.GetByStatusAsync(statusId);

                List<GetPackingsByStatusResponse> responseNotes = packingList.Select(packing => new GetPackingsByStatusResponse
                {
                    Client = new NoteCustomer
                    {
                        Cnpj = packing.Client.Cnpj,
                        SocialReason = packing.Client.SocialReason
                    },
                    ClientId = packing.ClientId,
                    Id = packing.Id,
                    Description = packing.Description,
                    Quantity = packing.Quantity,
                    Responsible = packing.Responsible,
                    Status = packing.Status,
                    CreatedAt = packing.CreatedAt,
                }).ToList();

                return Ok(responseNotes);
            }
            catch (Exception ex)
            {
                return Problem((ex.InnerException ?? ex).Message);
            }
        }

        [Route("GetUserLogedId")]
        [HttpGet]
        [Authorize(Roles = "Admin,Gerente,Funcionário")]
        public int GetUserLogedId()
        {
            // Access the JWT token from the Authorization header
            string authorizationHeader = HttpContext.Request.Headers["Authorization"];
            string token = authorizationHeader?.Replace("Bearer ", "");

            string keyJwt = _configuration["Jwt:Key"];


            //  Decode the JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(keyJwt);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
            ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            // Acess claim and get User Id
            var jsonWithUserId = claimsPrincipal.Claims.FirstOrDefault().ToString();

            //Convert Id to int
            return ExtractIdFromInputHelper.ExtractIdFromInput(jsonWithUserId);
            
        }

        [Route("ValidatePickingListItem")]
        [HttpPost]
        [Authorize(Roles = "Admin,Gerente,Funcionário")]
        public async Task<ActionResult> ValidatePickingListItem([FromBody] ValidatePickingListItemRequest data)
        {
            try
            {
                int id = GetUserLogedId();

                //await _pickingListHistoryService.Add(data.PickingListId, "", PickingListStatus.EmAtendimento, id);

                var pickingListItem = _pickingListItemService.Validate(data, id);

                List<int> ints = await _expeditionOrderRepository.GetOrdersIdsByPickingListAsync(data.PickingListId);

                HashSet<int> uniqueIds = new HashSet<int>();

                foreach (int i in ints)
                {
                    if (uniqueIds.Add(i))
                    {
                        await _expeditionOrderHistoryService.Add(i, "Separação Bipada", ExpeditionOrderStatus.BeepingPickingList, id);
                    }
                }

                return Ok(pickingListItem);
            }
            catch (Exception ex)
            {
                return Problem((ex.InnerException ?? ex).Message);
            }
        }

        [Route("ValidatePickingListItemByList")]
        [HttpPost]
        [Authorize(Roles = "Admin,Gerente,Funcionário")]
        public async Task<ActionResult> ValidatePickingListItemByList([FromBody] ValidatePickingListItemRequest[] data)
        {
            List<ValidatePickingListItemRequest> itemsWithError = new List<ValidatePickingListItemRequest>();
            int successCounter = 0;
            foreach (var item in data)
            {
                try
                {
                    int id = GetUserLogedId();
                    var pickingListItem = _pickingListItemService.Validate(item, id);

                    successCounter++;
                }
                catch
                {
                    itemsWithError.Add(item);
                }
            }

            return Ok(new ValidatePickingListItemByListResponse
            {
                Error = itemsWithError,
                Sent = data.Length,
                Success = successCounter
            });
        }

        [Route("GetPickingListById/{id}")]
        [HttpGet]
        [Authorize(Roles = "Admin,Gerente,Funcionário")]
        public ActionResult<PickingList> GetPickingListById(int id)
        {
            try
            {
                var pickingList = _pickingListRepository.GetById(id);
                pickingList.PickingListItems = pickingList.PickingListItems.OrderBy(pi => pi.Address).ToList();
                return Ok(pickingList);
            }
            catch (Exception ex)
            {
                return Problem((ex.InnerException ?? ex).Message);
            }
        }

        [Route("GetPackingById/{id}")]
        [HttpGet]
        [Authorize(Roles = "Admin,Gerente,Funcionário")]
        public async Task<ActionResult<Packing>> GetPackingById(int id)
        {
            try
            {
                return Ok(await _packingRepository.GetByIdAsync(id));
            }
            catch (Exception ex)
            {
                return Problem((ex.InnerException ?? ex).Message);
            }
        }

        [Route("GetNotesByAcessKey/{key}")]
        [HttpGet]
        [Authorize(Roles = "Admin,Gerente,Funcionário")]
        public ActionResult<ReceiptNote> GetNotesByDanfe(string key)
        {
            try
            {
                ReceiptNote note = _receiptNoteRepository.GetByAcessKey(key);

                return note is null ? throw new Exception("Receipt not found") : Ok(note);
            }
            catch (Exception ex)
            {
                return Problem((ex.InnerException ?? ex).Message);
            }
        }

        [Route("SendPackingImage/{id}")]
        [HttpPost]
        //[Authorize(Roles = "Admin,Gerente,Funcionário")]
        public async Task<ActionResult<PickingList>> GetSendPackingImageById(int id, List<IFormFile> image)
        {
            try
            {
                await _packingService.SendPackingImage(id, image);

                return Ok();
            }
            catch (Exception ex)
            {
                return Problem((ex.InnerException ?? ex).Message);
            }
        }

        [Route("ValidatePackingItem")]
        [HttpPost]
        [Authorize(Roles = "Admin,Gerente,Funcionário")]
        public async Task<ActionResult> ValidatePackingItem([FromBody] ValidatePackingItemRequest data)
        {
            try
            {
                int id = GetUserLogedId();

                var pickingListItem = await _packingItemService.Validate(data, id);

                return Ok(pickingListItem);
            }
            catch (Exception ex)
            {
                return Problem((ex.InnerException ?? ex).Message);
            }
        }

        [Route("CreateTransportationPerson")]
        [HttpPost]
        public async Task<ActionResult> CreateTransportationPerson([FromBody] CreateTransportationPersonRequest data)
        {
            try
            {
                TransportationPerson transportationPerson = _transportationPersonRepository.GetByCpf(data.Cpf);

                if (transportationPerson != null) throw new Exception("CPF já cadastrado");

                TransportationPerson newTransportationPerson = await _transportationPersonService.Generate(data);

                return Ok(newTransportationPerson);
            }
            catch (Exception ex)
            {
                return Problem((ex.InnerException ?? ex).Message);
            }
        }

        [Route("GetShippingCompanyDtoResponse")]
        [HttpGet]
        [Authorize(Roles = "Admin,Gerente,Funcionário")]
        public async Task<ActionResult> GetShippingCompanyDtoResponse()
        {
            try
            {
                List<ShippingCompany> companies = await _shippingCompanyService.GetAll();

                List<GetShippingCompanyResponse> DtoResponseList = new List<GetShippingCompanyResponse>();


                foreach (var company in companies)
                {
                    GetShippingCompanyResponse DtoResponse = new GetShippingCompanyResponse();
                    DtoResponse.Id = company.Id;
                    DtoResponse.Name = company.Name;

                    DtoResponseList.Add(DtoResponse);
                }

                return Ok(DtoResponseList);

            }
            catch (Exception ex)
            {
                return Problem((ex.InnerException ?? ex).Message);
            }
        }

        [Route("SignPackingListTransportation/{id}")]
        [HttpPost]
        public async Task<ActionResult> SignPackingListTransportation(int id, List<IFormFile> signImage, List<IFormFile> plateImage)
        {
            try
            {
                await _packingListTransportationService.SaveTransportPlate(id, plateImage);
            }
            catch (Exception ex)
            {
                return Problem((ex.InnerException ?? ex).Message);
            }

            try
            {
                int userId = GetUserLogedId();

                await _packingListTransportationService.SignTransportation(id, signImage, userId);

                return Ok();
            }
            catch (Exception ex)
            {
                return Problem((ex.InnerException ?? ex).Message);
            }
        }

        [Route("UpdateImagesTransportationPerson/{id}")]
        [HttpPost]
        public async Task<ActionResult<PickingList>> UpdateImagesTransportationPerson(int id, List<IFormFile> frontImage, List<IFormFile> backImage)
        {
            try
            {
                await _transportationPersonService.UpdateImage(id, frontImage, backImage);

                return Ok();
            }
            catch (Exception ex)
            {
                return Problem((ex.InnerException ?? ex).Message);
            }
        }

        [Route("SetPackingListTransportation")]
        [HttpPost]
        [Authorize(Roles = "Admin,Gerente,Funcionário")]
        public async Task<ActionResult<PackingListTransportationResponse>> SetPackingListTransportation([FromBody] PackingListTransportationRequest data)
        {
            try
            {
                if (data != null)
                {
                    int id = GetUserLogedId();
                    return Ok(await _packingListTransportationService.Add(data,id));
                }
                else
                {
                    throw new ArgumentNullException("Não foi possível adicionar o romaneio, tente novamente!");
                }

            }
            catch (Exception ex)
            {
                return Problem((ex.InnerException ?? ex).Message);
            }
        }

        [Route("AssociatePackingListTransportationAndOrder/{id}/{invoiceAccessKey}")]
        [HttpPost]
        [Authorize(Roles = "Admin,Gerente,Funcionário")]
        public async Task<ActionResult> AssociatePackingListTransportationAndOrder(int id, string invoiceAccessKey)
        {
            try
            {
                ExpeditionOrder order = _expeditionOrderRepository.GetByInvoiceAccessKey(invoiceAccessKey);

                if (order != null)
                {
                    if (order.PackingListTransportationId == 0 || order.PackingListTransportationId == null)
                    {
                        order.PackingListTransportationId = id;
                        if (order.Status == ExpeditionOrderStatus.Separated || order.Status == ExpeditionOrderStatus.Packed)
                        {
                            _expeditionOrderRepository.Update(order);
                            return Ok(order);
                        }
                        else
                        {
                            throw new Exception("Pedidos com esse status não podem ser associados a um romaneio, por favor, tente com outro pedido.");
                        }

                    }

                    throw new Exception("O pedido já está associado a um romaneio.");
                }

                throw new ArgumentNullException("Não foi possível encontrar o pedido com o id indicado, por favor, tente novamente!");

            }
            catch (Exception ex)
            {
                return Problem((ex.InnerException ?? ex).Message);
            }
        }

        [Route("GetOrderByPackingListTransportationId/{id}")]
        [HttpGet]
        [Authorize(Roles = "Admin,Gerente,Funcionário")]
        public ActionResult<List<ExpeditionOrder>> GetPackingListTranportationByStatus(int id)
        {
            try
            {
                List<ExpeditionOrder> orders = _expeditionOrderRepository.GetByPackingListTransportationId(id);

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return Problem((ex.InnerException ?? ex).Message);
            }
        }

        [Route("GetPackingListTranportationByStatus/{statusId}")]
        [HttpGet]
        [Authorize(Roles = "Admin,Gerente,Funcionário")]
        public ActionResult<List<PackingListTransportationResponse>> GetPackingListTranportationByStatus(PackingListTransportationStatus statusId)
        {
            try
            {
                List<Models.PackingListTransportation> packingListTransportations = _packingListTransportationRepository.GetByStatus(statusId);

                return Ok(packingListTransportations);
            }
            catch (Exception ex)
            {
                return Problem((ex.InnerException ?? ex).Message);
            }
        }


        [Route("AssociatePickingListAndCart/{pickingListId}/{cartId}")]
        [HttpPost]
        [Authorize(Roles = "Admin,Gerente,Funcionário")]
        public ActionResult<PickingList> AssociatePickingListAndCart(int pickingListId, int cartId)
        {
            try
            {
                PickingList picking = _pickingListRepository.GetById(pickingListId);

                if (picking.CartId > 0)
                {
                    throw new ArgumentException("Essa lista de escolha já está associada a um carrinho!");
                }
                else
                {
                    picking.CartId = cartId;

                    _pickingListRepository.Update(picking);

                    return Ok(picking);
                }

            }
            catch (Exception ex)
            {
                throw new ArgumentException("Não foi possível associar essa lista de seleção a este carrinho. Detalhe do erro: " + ex);
            }
        }



        [Route("GetTransportationPersonByCpf/{cpf}")]
        [HttpGet]
        [Authorize(Roles = "Admin,Gerente,Funcionário")]
        public ActionResult<TransportationPerson> GetTransportationPersonByCpf(string cpf)
        {
            try
            {
                TransportationPerson transportationPerson = _transportationPersonRepository.GetByCpf(cpf);

                return Ok(transportationPerson);

            }
            catch (Exception ex)
            {
                return Problem((ex.InnerException ?? ex).Message);
            }
        }


        [Route("GetShippingCompany")]
        [HttpGet]
        [Authorize(Roles = "Admin,Gerente,Funcionário")]
        public async Task<ActionResult<List<ShippingCompany>>> GetAllShippingCompany()
        {
            try
            {
                List<ShippingCompany> shipingCompanyList = await _shippingCompanyRepository.GetAll();

                shipingCompanyList = shipingCompanyList.Where(x => x.Active == Status.Ativo).ToList();

                return Ok(shipingCompanyList);
            }
            catch (Exception ex)
            {
                return Problem((ex.InnerException ?? ex).Message);
            }
        }

        [Route("ResetReceiptNote/{id}")]
        [HttpPost]
        [Authorize(Roles = "Admin,Gerente,Funcionário")]
        public ActionResult ResetReceiptNote(int id)
        {
            return Ok(_receiptNoteService.ResetReceiptNote(id));
        }
    }
}

