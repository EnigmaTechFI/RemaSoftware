using System;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;
using RemaSoftware.Domain.Data;
using RemaSoftware.WebApp.Models.OrderViewModel;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NLog;
using QRCoder;
using RemaSoftware.Domain.Constants;
using RemaSoftware.UtilityServices.Interface;
using RemaSoftware.WebApp.DTOs;
using RemaSoftware.WebApp.Hub;

namespace RemaSoftware.WebApp.Helper
{
    public class OrderHelper
    {
        private readonly IOrderService _orderService;
        private readonly IEmailService _emailService;
        private readonly IOperationService _operationService;
        private readonly ISubBatchService _subBatchService;
        private readonly IProductService _productService;
        private readonly IAPIFatturaInCloudService _apiFatturaInCloudService;
        private readonly ApplicationDbContext _dbContext;
        private readonly ProductionHub _productionHub;
        private readonly IAPIFatturaInCloudService _apiFatturaInCloud;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ISupplierService _supplierService;
        private readonly IPriceService _priceService;
        private IClientService _clientService;
        private readonly IConfiguration _configuration;

        public OrderHelper(IOrderService orderService, IAPIFatturaInCloudService apiFatturaInCloudService,
            ApplicationDbContext dbContext, IProductService productService, ISubBatchService subBatchService, IPriceService priceService,
            IOperationService operationService, IEmailService emailService, ProductionHub productionHub, IAPIFatturaInCloudService apiFatturaInCloud, ISupplierService supplierService, IClientService clientService, IConfiguration configuration)
        {
            _orderService = orderService;
            _apiFatturaInCloudService = apiFatturaInCloudService;
            _dbContext = dbContext;
            _productService = productService;
            _subBatchService = subBatchService;
            _operationService = operationService;
            _emailService = emailService;
            _productionHub = productionHub;
            _apiFatturaInCloud = apiFatturaInCloud;
            _supplierService = supplierService;
            _clientService = clientService;
            _configuration = configuration;
            _priceService = priceService;
        }

        public Ddt_In GetDdtInById(int id)
        {
            return _orderService.GetDdtInById(id);
        }

        public SubBatchMonitoringViewModel GetSubBatchMonitoring(int id, string url)
        {
            var test = _subBatchService.GetSubBatchById(id);
            return new SubBatchMonitoringViewModel()
            {
                DdtSupplierUrl = url,
                SubBatch = _subBatchService.GetSubBatchById(id),
                BasePathImages = $"{_configuration["ApplicationUrl"]}{_configuration["ImagesEndpoint"]}order/"
            };
        }

        public List<SubBatch> GetSubBatchesStatus(string status)
        {
            return _subBatchService.GetSubBatchesStatus(status);
        }

        public List<Ddt_In> GetAllDdtIn_NoPagination()
        {
            return _orderService.GetAllDdtIn();
        }

        public List<Ddt_In> GetAllDdtInActive_NoPagination()
        {
            return _orderService.GetDdtInActive();
        }

        public List<Ddt_In> GetAllDdtInEnded_NoPagination()
        {
            return _orderService.GetDdtInEnded();
        }

        public Ddt_In AddNewDdtIn(NewOrderViewModel model)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                { //TAG1
                    var newSubbatch = false;
                    var operationsSelected = model.OperationsSelected.Where(w => !w.StartsWith("0"))
                        .Select(s => int.Parse(s.Split('-').First())).ToList();
                    operationsSelected.Add(_operationService.GetOperationIdByName(OtherConstants.EXTRA));
                    operationsSelected.Add(_operationService.GetOperationIdByName(OtherConstants.COQ));
                    if (model.Price.Contains(","))
                    {
                        model.Ddt_In.Price_Uni = Convert.ToDecimal(model.Price.Replace(",", "."));
                    }
                    else
                    {
                        model.Ddt_In.Price_Uni = Convert.ToDecimal(model.Price);
                    }
                    var batchOperationList = new List<BatchOperation>();
                    var index = 0;
                    foreach (var operation in operationsSelected)
                    {
                        batchOperationList.Add(new BatchOperation()
                        {
                            OperationID = operation,
                            Ordering = index++
                        });
                    }

                    var batch = _orderService.GetBatchByProductIdAndOperationList(model.Ddt_In.ProductID,
                        operationsSelected);
                    model.Ddt_In.FC_Ddt_In_ID = _apiFatturaInCloudService.AddDdtInCloud(model.Ddt_In,
                        _productService.GetProductById(model.Ddt_In.ProductID).SKU);
                    if (batch == null)
                    {
                        var ddtList = new List<Ddt_In>();
                        ddtList.Add(model.Ddt_In);
                        var subBatches = new List<SubBatch>();
                        subBatches.Add(new SubBatch()
                        {
                            Ddts_In = ddtList,
                            Status = OrderStatusConstants.STATUS_ARRIVED,
                        });
                        newSubbatch = true;
                        _orderService.CreateBatch(new Batch()
                        {
                            SubBatches = subBatches,
                            BatchOperations = batchOperationList
                        });
                    }
                    else
                    {
                        var subBatchInStock = batch.SubBatches
                            .Where(s => s.Status == OrderStatusConstants.STATUS_ARRIVED).OrderByDescending(s => s.SubBatchID).FirstOrDefault();
                        if (subBatchInStock != null && !model.NewSubBatch)
                        {
                            subBatchInStock.Ddts_In.Add(model.Ddt_In);
                            _subBatchService.UpdateSubBatch(subBatchInStock);
                        }
                        else
                        {
                            var ddts = new List<Ddt_In>();
                            ddts.Add(model.Ddt_In);
                            var subBatch = new SubBatch()
                            {
                                BatchID = batch.BatchId,
                                Ddts_In = ddts,
                                Status = OrderStatusConstants.STATUS_ARRIVED,
                            };
                            _subBatchService.CreateSubBatch(subBatch);
                            newSubbatch = true;
                        }
                    }

                    if (model.Ddt_In.NumberMissingPiece != 0)
                    {
                        var product = _productService.GetProductById(model.Ddt_In.ProductID);
                        try
                        {
                            _emailService.SendEmailMissingPieces(product.Client.Email, model.Ddt_In.NumberMissingPiece,
                                model.Ddt_In.Number_Piece, model.Ddt_In.Code, product.Client.Name, product.SKU,
                                product.Name);
                        }
                        catch (Exception e)
                        {
                        }
                    }

                    transaction.Commit();
                    if(newSubbatch)
                        _productionHub.NewSubBatchInStock();
                    return model.Ddt_In;
                }
                catch (Exception e)
                {
                    if(model.Ddt_In.FC_Ddt_In_ID != "" && model.Ddt_In.FC_Ddt_In_ID != null)
                        _apiFatturaInCloud.DeleteOrder(model.Ddt_In.FC_Ddt_In_ID);
                    transaction.Rollback();
                    throw;
                }
            }
        }
        
        public Ddt_In AddDuplicateDdtIn(DuplicateOrderViewModel model)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {//TAG1
                    var newSubbatch = false;
                    var operationsSelected = model.OperationsSelected.Where(w => !w.StartsWith("0"))
                        .Select(s => int.Parse(s.Split('-').First())).ToList();
                    operationsSelected.Add(_operationService.GetOperationIdByName(OtherConstants.EXTRA));
                    operationsSelected.Add(_operationService.GetOperationIdByName(OtherConstants.COQ));
                    if (model.Price.Contains(","))
                    {
                        model.Ddt_In.Price_Uni = Convert.ToDecimal(model.Price.Replace(",", "."));
                    }
                    else
                    {
                        model.Ddt_In.Price_Uni = Convert.ToDecimal(model.Price);
                    }
                    var batchOperationList = new List<BatchOperation>();
                    var index = 0;
                    foreach (var operation in operationsSelected)
                    {
                        batchOperationList.Add(new BatchOperation()
                        {
                            OperationID = operation,
                            Ordering = index++
                        });
                    }

                    var batch = _orderService.GetBatchByProductIdAndOperationList(model.Ddt_In.ProductID,
                        operationsSelected);
                    model.Ddt_In.FC_Ddt_In_ID = _apiFatturaInCloudService.AddDdtInCloud(model.Ddt_In,
                        _productService.GetProductById(model.Ddt_In.ProductID).SKU);
                    if (batch == null)
                    {
                        var ddtList = new List<Ddt_In>();
                        ddtList.Add(model.Ddt_In);
                        var subBatches = new List<SubBatch>();
                        subBatches.Add(new SubBatch()
                        {
                            Ddts_In = ddtList,
                            Status = OrderStatusConstants.STATUS_ARRIVED,
                        });
                        newSubbatch = true;
                        _orderService.CreateBatch(new Batch()
                        {
                            SubBatches = subBatches,
                            BatchOperations = batchOperationList
                        });
                    }
                    else
                    {
                        var subBatchInStock = batch.SubBatches
                            .Where(s => s.Status == OrderStatusConstants.STATUS_ARRIVED).OrderByDescending(s => s.SubBatchID).FirstOrDefault();
                        if (subBatchInStock != null && !model.NewSubBatch)
                        {
                            subBatchInStock.Ddts_In.Add(model.Ddt_In);
                            _subBatchService.UpdateSubBatch(subBatchInStock);
                        }
                        else
                        {
                            var ddts = new List<Ddt_In>();
                            ddts.Add(model.Ddt_In);
                            var subBatch = new SubBatch()
                            {
                                BatchID = batch.BatchId,
                                Ddts_In = ddts,
                                Status = OrderStatusConstants.STATUS_ARRIVED,
                            };
                            _subBatchService.CreateSubBatch(subBatch);
                            newSubbatch = true;
                        }
                    }

                    if (model.Ddt_In.NumberMissingPiece != 0)
                    {
                        var product = _productService.GetProductById(model.Ddt_In.ProductID);
                        try
                        {
                            _emailService.SendEmailMissingPieces(product.Client.Email, model.Ddt_In.NumberMissingPiece,
                                model.Ddt_In.Number_Piece, model.Ddt_In.Code, product.Client.Name, product.SKU,
                                product.Name);
                        }
                        catch (Exception e)
                        {
                        }
                    }

                    transaction.Commit();
                    if(newSubbatch)
                        _productionHub.NewSubBatchInStock();
                    return model.Ddt_In;
                }
                catch (Exception e)
                {
                    if(model.Ddt_In.FC_Ddt_In_ID != "" && model.Ddt_In.FC_Ddt_In_ID != null)
                        _apiFatturaInCloud.DeleteOrder(model.Ddt_In.FC_Ddt_In_ID);
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void RegisterBatchAtCOQ(int subBatchId)
        {
            var subBatch = new SubBatch();
            try
            {
                subBatch = _subBatchService.GetSubBatchById(subBatchId);
                
            }
            catch (Exception e)
            {
                throw new Exception("Errore nel recupero del lotto.");
            }

            if (subBatch.OperationTimelines != null && subBatch.OperationTimelines.Where(s => s.MachineId != null).ToList().Any(s =>
                    s.MachineId == 99 && s.Status == OrderStatusConstants.STATUS_ARRIVED))
                throw new Exception("Lotto già registrato al controllo qualità.");
            if (subBatch.Status == OrderStatusConstants.STATUS_COMPLETED ||
                subBatch.Status == OrderStatusConstants.STATUS_COMPLETED)
                throw new Exception("Lotto già completato.");
            if (subBatch.Ddts_In.Sum(s => s.Number_Piece_Now) <= 0)
                throw new Exception("Nessun pezzo attualmente in azienda.");

            if (subBatch != null)
            {
                if (subBatch.OperationTimelines == null)
                    subBatch.OperationTimelines = new List<OperationTimeline>();
                var now = DateTime.Now;
                var batchOp =
                    subBatch.Batch.BatchOperations.SingleOrDefault(s => s.Operations.Name == OtherConstants.COQ) ??
                    new BatchOperation()
                    {
                        BatchID = subBatch.BatchID,
                        OperationID = _dbContext.Operations.SingleOrDefault(s => s.Name == OtherConstants.COQ)
                            .OperationID,
                        Ordering = subBatch.Batch.BatchOperations == null
                            ? 1
                            : subBatch.Batch.BatchOperations.Max(s => s.Ordering) + 1
                    };
                subBatch.OperationTimelines.Add(new OperationTimeline()
                {
                    SubBatchID = subBatchId,
                    BatchOperation = batchOp,
                    Status = OperationTimelineConstant.STATUS_WORKING,
                    MachineId = 99,
                    StartDate = now,
                    EndDate = now
                });
                subBatch.Status = OrderStatusConstants.STATUS_WORKING;
                subBatch.Ddts_In.ForEach(s => s.Status = OrderStatusConstants.STATUS_WORKING);
            }
            _subBatchService.UpdateSubBatch(subBatch);
        }

        public QualityControlViewModel GetQualityControlViewModel()
        {
            return new QualityControlViewModel()
            {
                OperationTimeLine = _subBatchService.GetSubBatchAtQualityControl() ?? new List<OperationTimeline>(),
                Label = _orderService.GetLastLabelOut()
            };
        }

        public int EndOrder(SubBatchToEndDto dto)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (dto == null)
                        throw new Exception("I dati inseriti non sono corretti.");
                    var subBatch = _subBatchService.GetSubBatchById(dto.SubBatchId);
                    if (dto.LostPieces + dto.WastePieces + dto.OkPieces + dto.ZamaPieces + dto.ResoScarto > subBatch.Ddts_In.Sum(s => s.Number_Piece_Now))
                        throw new Exception("Il totale dei pezzi inserito è maggiore dei pezzi attualmente in azienda.");
                    if (dto.LostPieces + dto.WastePieces + dto.OkPieces + dto.ZamaPieces + dto.ResoScarto<= 0)
                        throw new Exception("Nessun pezzo inserito.");
                    _orderService.CreateNewLabelOut(new Label()
                    {
                        Client = subBatch.Ddts_In[0].Product.Client.Name,
                        Date = DateTime.Now,
                        LostPieces = dto.LostPieces,
                        OkPieces = dto.OkPieces,
                        SKU = subBatch.Ddts_In[0].Product.SKU,
                        SubBatchCode = subBatch.SubBatchID,
                        WastePieces = dto.WastePieces,
                        ZamaPieces = dto.ZamaPieces,
                        ResoScarto = dto.ResoScarto
                    });
                    var now = DateTime.Now;
                    var ddt_out_id = 0;
                    var ddts_out = _orderService.GetDdtOutsByClientIdAndStatus(subBatch.Ddts_In[0].Product.ClientID,
                        DDTOutStatus.STATUS_PENDING);
                    if (ddts_out.Count == 0)
                    {
                        ddt_out_id = _orderService.CreateNewDdtOut(new Ddt_Out()
                        {
                            ClientID = subBatch.Ddts_In[0].Product.ClientID,
                            Date = now,
                            Status = DDTOutStatus.STATUS_PENDING
                        }).Ddt_Out_ID;
                    }
                    else
                    {
                        ddt_out_id = ddts_out[0].Ddt_Out_ID;
                    }

                        
                    foreach (var item in subBatch.Ddts_In.OrderBy(s => s.DataIn).ToList())
                    {
                        item.Ddt_Associations ??= new List<Ddt_Association>();
                        var missingPiecesEmitted = item.Ddt_Associations.Where(s => s.TypePieces == PiecesType.MANCANTI)
                            .Sum(s => s.NumberPieces);
                        if (item.NumberMissingPiece > 0 && missingPiecesEmitted < item.NumberMissingPiece)
                        {
                            item.Ddt_Associations.Add(new Ddt_Association()
                            {
                                Date = now,
                                Ddt_In_ID = item.Ddt_In_ID,
                                Ddt_Out_ID = ddt_out_id,
                                NumberPieces = item.NumberMissingPiece - missingPiecesEmitted,
                                TypePieces = PiecesType.MANCANTI
                            });
                        }

                        if (dto.OkPieces > 0)
                        {
                            if (item.Number_Piece_Now > dto.OkPieces)
                            {
                                item.Number_Piece_Now -= dto.OkPieces;
                                item.Status = OrderStatusConstants.STATUS_PARTIALLY_COMPLETED;
                                item.Ddt_Associations.Add(new Ddt_Association()
                                {
                                    Date = now,
                                    Ddt_In_ID = item.Ddt_In_ID,
                                    Ddt_Out_ID = ddt_out_id,
                                    NumberPieces = dto.OkPieces,
                                    TypePieces = PiecesType.BUONI
                                });
                                dto.OkPieces = 0;
                            }
                            else if (item.Number_Piece_Now > 0)
                            {
                                dto.OkPieces -= item.Number_Piece_Now;
                                item.Status = OrderStatusConstants.STATUS_COMPLETED;
                                item.DataEnd = now;
                                item.Ddt_Associations.Add(new Ddt_Association()
                                {
                                    Date = now,
                                    Ddt_In_ID = item.Ddt_In_ID,
                                    Ddt_Out_ID = ddt_out_id,
                                    NumberPieces = item.Number_Piece_Now,
                                    TypePieces = PiecesType.BUONI
                                });
                                item.Number_Piece_Now = 0;
                                if (dto.OkPieces == 0)
                                    break;
                            }
                        }

                        if (dto.LostPieces > 0)
                        {
                            if (item.Number_Piece_Now >= dto.LostPieces)
                            {
                                item.Number_Piece_Now -= dto.LostPieces;
                                item.Status = OrderStatusConstants.STATUS_PARTIALLY_COMPLETED;
                                item.Ddt_Associations.Add(new Ddt_Association()
                                {
                                    Date = now,
                                    Ddt_In_ID = item.Ddt_In_ID,
                                    Ddt_Out_ID = ddt_out_id,
                                    NumberPieces = dto.LostPieces,
                                    TypePieces = PiecesType.PERSI
                                });
                                dto.LostPieces = 0;
                            }
                            else if(item.Number_Piece_Now > 0)
                            {
                                dto.LostPieces -= item.Number_Piece_Now;
                                item.Ddt_Associations.Add(new Ddt_Association()
                                {
                                    Date = now,
                                    Ddt_In_ID = item.Ddt_In_ID,
                                    Ddt_Out_ID = ddt_out_id,
                                    NumberPieces = item.Number_Piece_Now,
                                    TypePieces = PiecesType.PERSI
                                });
                                item.Status = OrderStatusConstants.STATUS_COMPLETED;
                                item.Number_Piece_Now = 0;
                            }
                        }
                        
                        if (dto.WastePieces > 0)
                        {
                            if (item.Number_Piece_Now >= dto.WastePieces)
                            {
                                item.Number_Piece_Now -= dto.WastePieces;
                                item.Status = OrderStatusConstants.STATUS_PARTIALLY_COMPLETED;
                                item.Ddt_Associations.Add(new Ddt_Association()
                                {
                                    Date = now,
                                    Ddt_In_ID = item.Ddt_In_ID,
                                    Ddt_Out_ID = ddt_out_id,
                                    NumberPieces = dto.WastePieces,
                                    TypePieces = PiecesType.SCARTI
                                });
                                dto.WastePieces = 0;
                            }
                            else if(item.Number_Piece_Now > 0)
                            {
                                dto.WastePieces -= item.Number_Piece_Now;
                                item.Status = OrderStatusConstants.STATUS_COMPLETED;
                                item.Ddt_Associations.Add(new Ddt_Association()
                                {
                                    Date = now,
                                    Ddt_In_ID = item.Ddt_In_ID,
                                    Ddt_Out_ID = ddt_out_id,
                                    NumberPieces = item.Number_Piece_Now,
                                    TypePieces = PiecesType.SCARTI
                                });
                                item.Number_Piece_Now = 0;
                            }
                        }
                        
                        if (dto.ZamaPieces > 0)
                        {
                            if (item.Number_Piece_Now >= dto.ZamaPieces)
                            {
                                item.Number_Piece_Now -= dto.ZamaPieces;
                                item.Status = OrderStatusConstants.STATUS_PARTIALLY_COMPLETED;
                                item.Ddt_Associations.Add(new Ddt_Association()
                                {
                                    Date = now,
                                    Ddt_In_ID = item.Ddt_In_ID,
                                    Ddt_Out_ID = ddt_out_id,
                                    NumberPieces = dto.ZamaPieces,
                                    TypePieces = PiecesType.ZAMA
                                });
                                dto.ZamaPieces = 0;
                            }
                            else if(item.Number_Piece_Now > 0)
                            {
                                dto.ZamaPieces -= item.Number_Piece_Now;
                                item.Status = OrderStatusConstants.STATUS_COMPLETED;
                                item.Ddt_Associations.Add(new Ddt_Association()
                                {
                                    Date = now,
                                    Ddt_In_ID = item.Ddt_In_ID,
                                    Ddt_Out_ID = ddt_out_id,
                                    NumberPieces = item.Number_Piece_Now,
                                    TypePieces = PiecesType.ZAMA
                                });
                                item.Number_Piece_Now = 0;
                            }
                        }
                        
                        if (dto.ResoScarto > 0)
                        {
                            if (item.Number_Piece_Now >= dto.ResoScarto)
                            {
                                item.Number_Piece_Now -= dto.ResoScarto;
                                item.Status = OrderStatusConstants.STATUS_PARTIALLY_COMPLETED;
                                item.Ddt_Associations.Add(new Ddt_Association()
                                {
                                    Date = now,
                                    Ddt_In_ID = item.Ddt_In_ID,
                                    Ddt_Out_ID = ddt_out_id,
                                    NumberPieces = dto.ResoScarto,
                                    TypePieces = PiecesType.RESOSCARTO
                                });
                                dto.WastePieces = 0;
                            }
                            else if(item.Number_Piece_Now > 0)
                            {
                                dto.ResoScarto -= item.Number_Piece_Now;
                                item.Status = OrderStatusConstants.STATUS_COMPLETED;
                                item.Ddt_Associations.Add(new Ddt_Association()
                                {
                                    Date = now,
                                    Ddt_In_ID = item.Ddt_In_ID,
                                    Ddt_Out_ID = ddt_out_id,
                                    NumberPieces = item.Number_Piece_Now,
                                    TypePieces = PiecesType.RESOSCARTO
                                });
                                item.Number_Piece_Now = 0;
                            }
                        }
                    }

                    if (subBatch.Ddts_In.All(s => s.Number_Piece_Now + s.Number_Piece_ToSupplier == 0))
                    {
                        subBatch.Status = OrderStatusConstants.STATUS_COMPLETED;
                        foreach (var item in subBatch.OperationTimelines
                                     .Where(s => s.Status != OperationTimelineConstant.STATUS_COMPLETED).ToList())
                        {

                            if (item.MachineId == 99 && item.OperationTimelineID == dto.OperationTimeLineId)
                            {
                                item.EndDate = DateTime.Now;
                                item.UseForStatics = false;
                                item.Status = OperationTimelineConstant.STATUS_COMPLETED;
                            }
                            else if (item.MachineId == 99 && item.OperationTimelineID != dto.OperationTimeLineId)
                            {
                                continue;
                            }
                            else
                            {
                                item.UseForStatics = false;
                                item.Status = OperationTimelineConstant.STATUS_COMPLETED;
                            }

                        }
                    }

                    _subBatchService.UpdateSubBatch(subBatch);
                    transaction.Commit();
                    return subBatch.SubBatchID;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }

        public List<Ddt_Out> GetDdtsInDelivery()
        {
            return _orderService.GetDdtOutsByStatus(DDTOutStatus.STATUS_PENDING);
        }
        
        public async Task<string> EmitDDT(int id)
        {
            (string, int, string) control = (null, 0, null);
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var ddtOut = _orderService.GetDdtOutById(id);
                
                    foreach (var item in ddtOut.Ddt_Associations)
                    {
                        item.Ddt_In.Status = (item.Ddt_In.Number_Piece_Now + item.Ddt_In.Number_Piece_ToSupplier == 0)
                            ? OrderStatusConstants.STATUS_DELIVERED
                            : item.Ddt_In.Status;

                        item.Ddt_In.SubBatch.Status = item.Ddt_In.SubBatch.Ddts_In.All(s => s.Number_Piece_Now + s.Number_Piece_ToSupplier == 0)
                            ? OrderStatusConstants.STATUS_DELIVERED
                            : item.Ddt_In.SubBatch.Status;
                    }

                    if (ddtOut.Ddt_Associations.Any(s => s.Ddt_In.PriceIsPending))
                    {
                        var ddts = string.Join(" | ", ddtOut.Ddt_Associations
                            .Where(item => item.Ddt_In.PriceIsPending && item.TypePieces == PiecesType.BUONI)
                            .Select(item => item.Ddt_In.Code));

                        throw new Exception($"Attenzione contattare l'amministrazione! Le seguenti DDT hanno il prezzo da confermare: {ddts}");
                    }

                    var result = await _apiFatturaInCloudService.CreateDdtInCloud(ddtOut);
                    control = result;
                    ddtOut.Status = DDTOutStatus.STATUS_EMITTED;
                    ddtOut.FC_Ddt_Out_ID = result.Item2;
                    ddtOut.Url = result.Item1;
                    ddtOut.Code = result.Item3;
                    await _orderService.UpdateDdtOut(ddtOut);

                    transaction.Commit();

                    return result.Item1;
                }
                catch (Exception e)
                {
                    if (control.Item1 != null)
                    {
                        _apiFatturaInCloudService.DeleteDdtInCloudById(control.Item2);
                    }

                    transaction.Rollback();
                    throw;
                }
            }
        }

        public DDTEmittedViewModel GetDDTEmittedViewModel()
        {
            var emitted = _orderService.GetDdtOutsByStatus(DDTOutStatus.STATUS_EMITTED);
            var emittedDto = new List<DDTEmittedDto>();
            foreach (var item in emitted)
            {
                var ddt = new DDTEmittedDto()
                {
                    Id = item.Ddt_Out_ID,
                    Url = item.Url,
                    NumberPieces = item.Ddt_Associations.Sum(s => s.NumberPieces),
                    DdtWithPieces = new List<(string, int)>(),
                    Client = item.Ddt_Associations[0].Ddt_In.Product.Client.Name,
                    Date = item.Date,
                    Code = item.Code
                };
                foreach (var entity in item.Ddt_Associations)
                {
                    ddt.DdtWithPieces.Add(new (entity.Ddt_In.Code, entity.NumberPieces));
                }
                emittedDto.Add(ddt);
            }
            return new DDTEmittedViewModel()
            {
                DdtOuts = emittedDto
            };
        }

        public int DeleteDDT(int id)
        {
            var ddt = _orderService.GetDdtOutsById(id);
            _apiFatturaInCloudService.DeleteDdtInCloudById(ddt.FC_Ddt_Out_ID);
            ddt.FC_Ddt_Out_ID = 0;
            ddt.Url = "";
            ddt.Status = DDTOutStatus.STATUS_PENDING;
            _orderService.UpdateDdtOut(ddt);
            return id;
        }

        public Ddt_In EditDdtIn(NewOrderViewModel model)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var ddt = _orderService.GetDdtInById(model.Ddt_In.Ddt_In_ID);
                    ddt.Code = model.Ddt_In.Code;
                    ddt.Number_Piece_Now = model.Ddt_In.Number_Piece_Now;
                    ddt.Number_Piece = model.Ddt_In.Number_Piece;
                    ddt.DataOut = DateTime.Parse(model.Date, new CultureInfo("it-IT"));
                    if (model.Ddt_In.NumberMissingPiece > 0 &&
                        ddt.NumberMissingPiece != model.Ddt_In.NumberMissingPiece)
                    {
                        var product = ddt.Product;
                        try
                        {
                            _emailService.SendEmailMissingPieces(product.Client.Email,  model.Ddt_In.NumberMissingPiece - ddt.NumberMissingPiece,
                                model.Ddt_In.Number_Piece, model.Ddt_In.Code, product.Client.Name, product.SKU,
                                product.Name);
                        }
                        catch (Exception e)
                        {
                        }
                    }
                    ddt.NumberMissingPiece = model.Ddt_In.NumberMissingPiece;
                    if (model.Price.Contains(","))
                    {
                        ddt.Price_Uni = Convert.ToDecimal( model.Price.Replace(",", "."));
                    }
                    else
                    {
                        ddt.Price_Uni = Convert.ToDecimal( model.Price);
                    }
                    ddt.Description = model.Ddt_In.Description;
                    ddt.Note = model.Ddt_In.Note;
                    var updatedDDT = _orderService.UpdateDDtIn(ddt);
                    _apiFatturaInCloudService.EditDdtInCloud(updatedDDT, ddt.Product.SKU);
                    transaction.Commit();
                    return updatedDDT;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public PartialDDTViewModel GetPartialDDTViewModel(int id, int clientId)
        {
            var ddt = _orderService.GetDdtOutById(id);
            var partialDDtDtos = new List<PartialDdtDto>();
            foreach (var item in ddt.Ddt_Associations)
            {
                partialDDtDtos.Add(new PartialDdtDto()
                {
                    DdtAssociation = item,
                    ToEmit = false
                });
            }

            return new PartialDDTViewModel()
            {
                DdtId = id,
                PartialDdtDtos = partialDDtDtos,
                ClientId = clientId
            };
        }

        public async Task<string> EmitPartialDdtIn(PartialDDTViewModel model)
        {
            try
            {
                if (model.PartialDdtDtos.All(s => s.ToEmit))
                    return await EmitDDT(model.DdtId);
                if (model.PartialDdtDtos.Any(s => s.ToEmit))
                {
                    var ddtOut = _orderService.CreateDDTOut(new Ddt_Out()
                    {
                        ClientID = model.ClientId,
                        Date = DateTime.Now,
                        Url = "",
                        Status = DDTOutStatus.STATUS_PENDING
                    });
                    var partial = model.PartialDdtDtos.Where(s => s.ToEmit).ToList();
                    foreach (var item in partial)
                    {
                        _orderService.UpdateDdtAssociationByIdWithNewDdtOut(item.DdtAssociation.ID, ddtOut.Ddt_Out_ID);
                    }
                    var result = await EmitDDT(ddtOut.Ddt_Out_ID);
                    return result;
                }
                return "";
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public StockSummaryViewModel GetStockSummaryViewModel()
        {
            var subBatches = _subBatchService.GetSubBatchesStatusForOrderSummary(OrderStatusConstants.STATUS_ARRIVED);
            var orderSB = new List<(SubBatch, long)>();
            foreach (var item in subBatches)
            {
                long Totvalue = 0;
                foreach (var ddt in item.Ddts_In)
                {
                    long value = 0;
                    value = Int64.Parse(GetTimestamp(ddt.DataOut)) / ddt.Priority;
                    if (ddt.IsPrompted)
                        value /= OtherConstants.PROMPTVALUE;
                    Totvalue += value;
                }

                Totvalue /= subBatches.Count; 
                orderSB.Add((item, Totvalue));
            }
            
            return new StockSummaryViewModel()
            {
                SubBatches = orderSB.OrderBy(s => s.Item2).Select(s => s.Item1).ToList()
            };
        }
        
        private static String GetTimestamp(DateTime value)
        {  
            return value.ToString("yyyyMMddHHmmssffff");
        }

        public int DeleteOrder(int ddtId)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var ddt = _orderService.GetDdtInById(ddtId);
                    if(ddt.Ddt_Associations != null && ddt.Ddt_Associations.Count > 0)
                        _orderService.DeleteDDTAssociations(ddt.Ddt_Associations);
                    _orderService.DeleteDDT(ddt);
                    if (ddt.SubBatch.Ddts_In.Count == 0)
                        _orderService.DeleteSubBatch(ddt.SubBatch);
                    _apiFatturaInCloud.DeleteOrder(ddt.FC_Ddt_In_ID);
                    transaction.Commit();
                    return ddtId;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }

        public void StockVariation(int id, int pieces)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var ddt = _orderService.GetDdtInById(id);
                    if (ddt == null)
                        throw new Exception("DDT non torvata. Si prega di riporovare.");
                    if (pieces < 0)
                        throw new Exception("Valore inserito non valido");
                    var diff = pieces - ddt.Number_Piece_Now;
                    ddt.Number_Piece_Now = pieces;
                    ddt.Number_Piece += diff;
                    ddt.Status = pieces == 0 ? OrderStatusConstants.STATUS_COMPLETED : ddt.Status;
                    _orderService.UpdateDDtIn(ddt);
                    _apiFatturaInCloud.EditDdtInCloud(ddt, ddt.Product.SKU);
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }

        public string CreateQRCode(int ddtSubBatchId)
        {
            QRCodeGenerator QrGenerator = new QRCodeGenerator();
            QRCodeData QrCodeInfo = QrGenerator.CreateQrCode($"URL:{ddtSubBatchId}", QRCodeGenerator.ECCLevel.Q);
            BitmapByteQRCode QrCode = new BitmapByteQRCode(QrCodeInfo);
            byte[] BitmapArray = QrCode.GetGraphic(60);
            return string.Format("data:image/png;base64,{0}", Convert.ToBase64String(BitmapArray));
        }

        public void DeleteDdtAssociation(int id)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var ddtAssociation = _orderService.GetDDTAssociationById(id);
                    switch (ddtAssociation.TypePieces)
                    {
                        case PiecesType.BUONI:
                            ddtAssociation.Ddt_In.Number_Piece_Now += ddtAssociation.NumberPieces;
                            break;
                        case PiecesType.PERSI:
                            ddtAssociation.Ddt_In.Number_Piece_Now += ddtAssociation.NumberPieces;
                            ddtAssociation.Ddt_In.NumberLostPiece -= ddtAssociation.NumberPieces;
                            break;
                        case PiecesType.SCARTI: 
                            ddtAssociation.Ddt_In.Number_Piece_Now += ddtAssociation.NumberPieces;
                            ddtAssociation.Ddt_In.NumberWastePiece -= ddtAssociation.NumberPieces;
                            break;
                        case PiecesType.ZAMA: 
                            ddtAssociation.Ddt_In.Number_Piece_Now += ddtAssociation.NumberPieces;
                            ddtAssociation.Ddt_In.NumberZama -= ddtAssociation.NumberPieces;
                            break;
                        case PiecesType.RESOSCARTO: 
                            ddtAssociation.Ddt_In.Number_Piece_Now += ddtAssociation.NumberPieces;
                            ddtAssociation.Ddt_In.NumberReturnDiscard -= ddtAssociation.NumberPieces;
                            break;
                    }

                    ddtAssociation.Ddt_In.SubBatch.Status = OrderStatusConstants.STATUS_WORKING;
                    ddtAssociation.Ddt_In.Status =
                        ddtAssociation.Ddt_In.Number_Piece == ddtAssociation.Ddt_In.Number_Piece_Now
                            ? OrderStatusConstants.STATUS_WORKING
                            : OrderStatusConstants.STATUS_PARTIALLY_COMPLETED;
                    _orderService.UpdateDDtIn(ddtAssociation.Ddt_In);
                    _orderService.DeleteDDTAssociation(ddtAssociation);
                    if (ddtAssociation.Ddt_Out.Ddt_Associations.Count == 0)
                    {
                        _orderService.DeleteDDTOut(ddtAssociation.Ddt_Out);
                    }
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    Logger.Error(e, e.Message);
                    throw new Exception("Errore durante l'annullamento della DDT.");
                }
            }
        }

        public ExitToSupplierViewModel GetExitToSupplierViewModel(int id)
        {
            var subBatch = _subBatchService.GetSubBatchById(id);
            return new ExitToSupplierViewModel()
            {
                SubBatch = _subBatchService.GetSubBatchById(id),
                Suppliers = _supplierService.GetSuppliers(),
                BatchOperations = subBatch.Batch.BatchOperations.Where(s => s.Operations.Name != OtherConstants.COQ).ToList(), 
            };
        }

        public string RegisterExitSubBatch(ExitToSupplierViewModel model)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var subBatch = _subBatchService.GetSubBatchById(model.SubBatch.SubBatchID);
                    if (model.DdtSupplier.Number_Piece > subBatch.Ddts_In.Sum(s => s.Number_Piece_Now))
                        throw new Exception("Pezzi inseriti maggiore di quelli attuali.");
                    if (subBatch.Status == OrderStatusConstants.STATUS_ARRIVED)
                    {
                        subBatch.Status = OrderStatusConstants.STATUS_WORKING;
                        _subBatchService.UpdateSubBatch(subBatch);
                    }
                    var now = DateTime.Now;
                    model.DdtSupplier.Status = OrderStatusConstants.STATUS_WORKING;
                    model.DdtSupplier.DataOut = now;
                    model.DdtSupplier.OperationTimeline = new OperationTimeline()
                    {
                        BatchOperationID = model.BatchOperationID,
                        StartDate = now,
                        EndDate = now,
                        Status = OperationTimelineConstant.STATUS_WORKING,
                        SubBatchID = model.SubBatch.SubBatchID
                    };
                    var totalPieces = model.DdtSupplier.Number_Piece;
                    var ddtSupplierAssociations = new List<DDT_Supplier_Association>();
                    foreach (var item in subBatch.Ddts_In.OrderBy(s => s.DataIn))
                    {
                        if (item.Number_Piece_Now > totalPieces)
                        {
                            item.Number_Piece_Now -= totalPieces;
                            item.Number_Piece_ToSupplier += totalPieces;
                            item.Status = item.Status == OrderStatusConstants.STATUS_ARRIVED ? OrderStatusConstants.STATUS_WORKING : item.Status;
                            _orderService.UpdateDDtIn(item);
                            ddtSupplierAssociations.Add(new DDT_Supplier_Association()
                            {
                                Ddt_In = item,
                                Ddt_In_ID = item.Ddt_In_ID,
                                NumberPieces = totalPieces,
                            });
                            break;
                        }
                        else
                        {
                            if (item.Number_Piece_Now > 0)
                            {
                                totalPieces -= item.Number_Piece_Now;
                                item.Number_Piece_ToSupplier += item.Number_Piece_Now;
                                ddtSupplierAssociations.Add(new DDT_Supplier_Association()
                                {
                                    Ddt_In = item,
                                    Ddt_In_ID = item.Ddt_In_ID,
                                    NumberPieces = item.Number_Piece_Now
                                });
                                item.Number_Piece_Now = 0;
                                item.Status = item.Status == OrderStatusConstants.STATUS_ARRIVED ? OrderStatusConstants.STATUS_WORKING : item.Status;
                                _orderService.UpdateDDtIn(item);
                                if (totalPieces <= 0)
                                    break;
                            }
                        }
                    }

                    var ddtSupplier = model.DdtSupplier;
                    ddtSupplier.DdtSupplierAssociations = ddtSupplierAssociations;
                    var supplier = _supplierService.GetSupplierById(model.DdtSupplier.SupplierID);
                    var result = _apiFatturaInCloud.CreateDdtSupplierCloud(ddtSupplier, supplier);
                    ddtSupplier.Code = result.Item2;
                    ddtSupplier.FC_Ddt_Supplier_ID = result.Item3.ToString();
                    ddtSupplier.Url = result.Item1;
                    _orderService.CreateNewDdtSupplier(ddtSupplier);
                    transaction.Commit();
                    return result.Item1;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    Logger.Error(e, e.Message);
                    throw new Exception(e.Message);
                }
            }
            
        }

        public BatchToSupplierViewModel GetBatchToSupplierViewModel()
        {
            return new BatchToSupplierViewModel()
            {
                Ddt_Suppliers = _subBatchService.GetSubBatchToSupplier()
            };
        }

        public ReloadSubBatchFromSupplierViewModel GetReloadSubBatchFromSupplierViewModel(int id)
        {
            var DDTSupplierPieces = _orderService.GetDdtSupplierById(id).DdtSupplierAssociations.Sum(s => s.Ddt_In.Number_Piece_ToSupplier);
            return new ReloadSubBatchFromSupplierViewModel()
            {
                DDTSupplierId = id,
                DDTSupplierPieces = DDTSupplierPieces
            };
        }

        public void ReloadSubBatchFromSupplier(ReloadSubBatchFromSupplierViewModel model)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    
                    var ddtSupplier = _orderService.GetDdtSupplierById(model.DDTSupplierId);
                    if (model.LostPieces + model.WastePieces + model.OkPieces + model.ZamaPieces + model.ResoScarto  > ddtSupplier.Number_Piece)
                        throw new Exception("Il totale dei pezzi inserito è maggiore dei pezzi attualmente in azienda.");
                    if (model.LostPieces + model.WastePieces + model.OkPieces + model.ZamaPieces + model.ResoScarto <= 0)
                        throw new Exception("Nessun pezzo inserito.");
                    ddtSupplier.NumberReInPiece += model.OkPieces;
                    ddtSupplier.NumberLostPiece += model.LostPieces;
                    ddtSupplier.NumberWastePiece += model.WastePieces;
                    ddtSupplier.NumberZamaPiece += model.ZamaPieces;
                    ddtSupplier.NumberResoScarto += model.ResoScarto;
                    ddtSupplier.Number_Piece -= (model.OkPieces + model.LostPieces + model.WastePieces + model.ZamaPieces + model.ResoScarto);
                    var ddts = new List<Ddt_In>();
                    
                    foreach (var item in ddtSupplier.DdtSupplierAssociations)
                    {
                        if (item.Ddt_In.Number_Piece_ToSupplier > 0)
                        {
                            ddts.Add(item.Ddt_In);
                        }
                    }
                    
                    ddts = ddts.OrderBy(s => s.DataIn).ToList();
                    var now = DateTime.Now;
                    var ddt_out_id = 0;
                    var ddts_out = _orderService.GetDdtOutsByClientIdAndStatus(ddts[0].Product.ClientID, DDTOutStatus.STATUS_PENDING);
                    if (ddts_out.Count == 0)
                    {
                        ddt_out_id = _orderService.CreateNewDdtOut(new Ddt_Out()
                        {
                            ClientID = ddts[0].Product.ClientID,
                            Date = now,
                            Status = DDTOutStatus.STATUS_PENDING
                        }).Ddt_Out_ID;
                    }
                    else
                    {
                        ddt_out_id = ddts_out[0].Ddt_Out_ID;
                    }

                    foreach (var item in ddts)
                    {
                        item.Ddt_Associations ??= new List<Ddt_Association>();
                        
                        if (item.Number_Piece_ToSupplier >= model.OkPieces + model.WastePieces + model.LostPieces + model.ZamaPieces + model.ResoScarto)
                        {
                            item.Number_Piece_Now += model.OkPieces;
                            item.NumberLostPiece += model.LostPieces;
                            item.NumberWastePiece += model.WastePieces;
                            item.NumberZama += model.ZamaPieces;
                            item.NumberReturnDiscard += model.ResoScarto;
                            item.Number_Piece_ToSupplier -= model.OkPieces + model.WastePieces + model.LostPieces + model.ZamaPieces + model.ResoScarto;
                            if (model.LostPieces > 0)
                            {
                                item.Ddt_Associations.Add(new Ddt_Association()
                                {
                                    Date = now,
                                    Ddt_In_ID = item.Ddt_In_ID,
                                    Ddt_Out_ID = ddt_out_id,
                                    NumberPieces = model.LostPieces,
                                    TypePieces = PiecesType.PERSI
                                });
                            }

                            if (model.WastePieces > 0)
                            {
                                item.Ddt_Associations.Add(new Ddt_Association()
                                {
                                    Date = now,
                                    Ddt_In_ID = item.Ddt_In_ID,
                                    Ddt_Out_ID = ddt_out_id,
                                    NumberPieces = model.WastePieces,
                                    TypePieces = PiecesType.SCARTI
                                });
                            }
                            
                            if (model.ZamaPieces > 0)
                            {
                                item.Ddt_Associations.Add(new Ddt_Association()
                                {
                                    Date = now,
                                    Ddt_In_ID = item.Ddt_In_ID,
                                    Ddt_Out_ID = ddt_out_id,
                                    NumberPieces = model.ZamaPieces,
                                    TypePieces = PiecesType.ZAMA
                                });
                            }
                            
                            if (model.ResoScarto > 0)
                            {
                                item.Ddt_Associations.Add(new Ddt_Association()
                                {
                                    Date = now,
                                    Ddt_In_ID = item.Ddt_In_ID,
                                    Ddt_Out_ID = ddt_out_id,
                                    NumberPieces = model.ResoScarto,
                                    TypePieces = PiecesType.RESOSCARTO
                                });
                            }
                            
                            model.OkPieces = 0;
                            model.LostPieces = 0;
                            model.WastePieces = 0;
                            model.ZamaPieces = 0;
                            model.ResoScarto = 0;
                            break;
                        }
                        else
                        {
                            if (model.LostPieces > 0)
                            {
                                var lost = 0;
                                if (item.Number_Piece_ToSupplier >= model.LostPieces)
                                {
                                    lost = model.LostPieces;
                                    item.NumberLostPiece += lost;
                                    item.Number_Piece_ToSupplier -= model.LostPieces;
                                    model.LostPieces = 0;
                                }
                                else
                                {
                                    lost = item.Number_Piece_ToSupplier;
                                    item.NumberLostPiece += lost;
                                    model.LostPieces -= item.Number_Piece_ToSupplier;
                                    item.Number_Piece_ToSupplier = 0;
                                }

                                if (lost > 0)
                                {
                                    item.Ddt_Associations.Add(new Ddt_Association()
                                    {
                                        Date = now,
                                        Ddt_In_ID = item.Ddt_In_ID,
                                        Ddt_Out_ID = ddt_out_id,
                                        NumberPieces = lost,
                                        TypePieces = PiecesType.PERSI
                                    });
                                }
                            }

                            if (model.WastePieces > 0)
                            {
                                var waste = 0;
                                if (item.Number_Piece_ToSupplier >= model.WastePieces)
                                {
                                    waste = model.WastePieces;
                                    item.NumberWastePiece += waste;
                                    item.Number_Piece_ToSupplier -= model.WastePieces;
                                    model.WastePieces = 0;
                                }
                                else
                                {
                                    waste = item.Number_Piece_ToSupplier;
                                    item.NumberWastePiece += waste;
                                    model.WastePieces -= item.Number_Piece_ToSupplier;
                                    item.Number_Piece_ToSupplier = 0;
                                }

                                if (waste > 0)
                                {
                                    item.Ddt_Associations.Add(new Ddt_Association()
                                    {
                                        Date = now,
                                        Ddt_In_ID = item.Ddt_In_ID,
                                        Ddt_Out_ID = ddt_out_id,
                                        NumberPieces = waste,
                                        TypePieces = PiecesType.SCARTI
                                    });
                                }
                            }
                            
                            if (model.ZamaPieces > 0)
                            {
                                var zama = 0;
                                if (item.Number_Piece_ToSupplier >= model.ZamaPieces)
                                {
                                    zama = model.ZamaPieces;
                                    item.NumberZama += zama;
                                    item.Number_Piece_ToSupplier -= model.ZamaPieces;
                                    model.ZamaPieces = 0;
                                }
                                else
                                {
                                    zama = item.Number_Piece_ToSupplier;
                                    item.NumberZama += zama;
                                    model.ZamaPieces -= item.Number_Piece_ToSupplier;
                                    item.Number_Piece_ToSupplier = 0;
                                }

                                if (zama > 0)
                                {
                                    item.Ddt_Associations.Add(new Ddt_Association()
                                    {
                                        Date = now,
                                        Ddt_In_ID = item.Ddt_In_ID,
                                        Ddt_Out_ID = ddt_out_id,
                                        NumberPieces = zama,
                                        TypePieces = PiecesType.ZAMA
                                    });
                                }
                            }
                            
                            if (model.ResoScarto > 0)
                            {
                                var resoScarto = 0;
                                if (item.Number_Piece_ToSupplier >= model.ResoScarto)
                                {
                                    resoScarto = model.ResoScarto;
                                    item.NumberReturnDiscard += resoScarto;
                                    item.Number_Piece_ToSupplier -= model.ResoScarto;
                                    model.ResoScarto = 0;
                                }
                                else
                                {
                                    resoScarto = item.Number_Piece_ToSupplier;
                                    item.NumberWastePiece += resoScarto;
                                    model.ResoScarto -= item.Number_Piece_ToSupplier;
                                    item.Number_Piece_ToSupplier = 0;
                                }

                                if (resoScarto > 0)
                                {
                                    item.Ddt_Associations.Add(new Ddt_Association()
                                    {
                                        Date = now,
                                        Ddt_In_ID = item.Ddt_In_ID,
                                        Ddt_Out_ID = ddt_out_id,
                                        NumberPieces = resoScarto,
                                        TypePieces = PiecesType.RESOSCARTO
                                    });
                                }
                            }

                            if (model.OkPieces > 0)
                            {
                                if (item.Number_Piece_ToSupplier >= model.OkPieces)
                                {
                                    item.Number_Piece_ToSupplier -= model.OkPieces;
                                    item.Number_Piece_Now += model.OkPieces;
                                }
                                else
                                {
                                    model.OkPieces -= item.Number_Piece_ToSupplier;
                                    item.Number_Piece_Now += item.Number_Piece_ToSupplier;
                                    item.Number_Piece_ToSupplier = 0;
                                }
                            }
                        }
                    }

                    if (model.OkPieces + model.LostPieces + model.WastePieces + model.ZamaPieces + model.ResoScarto > 0)
                        throw new Exception("Errore di ricarico. Contattare gli sviluppatori.");
                    _orderService.UpdateDDtInRange(ddts);
                    if (ddtSupplier.Number_Piece == 0)
                    {
                        ddtSupplier.OperationTimeline.Status = OperationTimelineConstant.STATUS_COMPLETED;
                        ddtSupplier.DataReIn = now;
                        ddtSupplier.Status = OrderStatusConstants.STATUS_COMPLETED;
                    }
                    _orderService.UpdateDDtSupplier(ddtSupplier);
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    Logger.Error(e, e.Message);
                    throw new Exception("Errore durante il ricarico.");
                }
            }
        }

        public DuplicateOrderViewModel GetDuplicateOrderViewModelForDuplicate(int id)
        {
            var subBatch = _subBatchService.GetSubBatchById(id);
            
            var op = new List<string>();
            var prices = _priceService.GetAllPricesByProductId(subBatch.Ddts_In[0].Product.ProductID);
            var operationIds = subBatch.Batch.BatchOperations.Where(s => s.Operations.Name != OtherConstants.COQ && s.Operations.Name != OtherConstants.EXTRA).Select(s => s.OperationID).ToList();
            
            foreach (var item in subBatch.Batch.BatchOperations.Where(s => s.Operations.Name != OtherConstants.COQ && s.Operations.Name != OtherConstants.EXTRA).ToList())
            {
                op.Add($"{item.OperationID}-{item.Operations.Name}");
            }
            var filteredPrices = new List<Price>();

            filteredPrices = prices.Where(price => operationIds.All(opId => price.PriceOperation.Any(po => po.OperationID == opId))).ToList();

            if (filteredPrices == null || filteredPrices.Count == 0)
            {
                var priceVal = "0.000";
                filteredPrices.Add(new Price() { PriceVal = Convert.ToDecimal(priceVal) });
            }

            return new DuplicateOrderViewModel
            {
                Clients = _clientService.GetAllClients(),
                Price = filteredPrices[0].PriceVal.ToString(),
                Ddt_In = new Ddt_In()
                {
                    ProductID = subBatch.Ddts_In[0].ProductID
                },
                OperationsSelected = op
            };
        }
    }

}