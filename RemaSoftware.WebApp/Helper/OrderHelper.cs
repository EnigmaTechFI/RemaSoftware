using System;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;
using RemaSoftware.Domain.Data;
using UtilityServices.Dtos;
using RemaSoftware.WebApp.Models.OrderViewModel;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RemaSoftware.Domain.Constants;
using RemaSoftware.UtilityServices.Dtos;
using RemaSoftware.UtilityServices.Interface;
using RemaSoftware.WebApp.DTOs;

namespace RemaSoftware.WebApp.Helper
{
    public class OrderHelper
    {
        private readonly IOrderService _orderService;
        private readonly IOperationService _operationService;
        private readonly ISubBatchService _subBatchService;
        private readonly IProductService _productService;
        private readonly IAPIFatturaInCloudService _apiFatturaInCloudService;
        private readonly ApplicationDbContext _dbContext;

        public OrderHelper(IOrderService orderService, IAPIFatturaInCloudService apiFatturaInCloudService, ApplicationDbContext dbContext, IProductService productService, ISubBatchService subBatchService, IOperationService operationService)
        {
            _orderService = orderService;
            _apiFatturaInCloudService = apiFatturaInCloudService;
            _dbContext = dbContext;
            _productService = productService;
            _subBatchService = subBatchService;
            _operationService = operationService;
        }

        public Ddt_In GetDdtInById(int id)
        {
            return _orderService.GetDdtInById(id);
        }

        public SubBatchMonitoringViewModel GetSubBatchMonitoring(int id)
        {
            return new SubBatchMonitoringViewModel()
            {
                SubBatch = _subBatchService.GetSubBatchById(id)
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
                {
                    var operationsSelected = model.OperationsSelected.Where(w => !w.StartsWith("0"))
                        .Select(s => int.Parse(s.Split('-').First())).ToList();
                    operationsSelected.Add(_operationService.GetOperationIdByName(OtherConstants.EXTRA));
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
                            Status = "A",
                            NumberPieces = model.Ddt_In.Number_Piece,
                        });
                        _orderService.CreateBatch(new Batch()
                        {
                            SubBatches = subBatches,
                            BatchOperations = batchOperationList
                        });
                    }
                    else
                    {
                        var subBatchInStock = batch.SubBatches
                            .Where(s => s.Status == OrderStatusConstants.STATUS_ARRIVED).SingleOrDefault();
                        if (subBatchInStock != null)
                        {
                            subBatchInStock.Ddts_In.Add(model.Ddt_In);
                            subBatchInStock.NumberPieces += model.Ddt_In.Number_Piece;
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
                                NumberPieces = model.Ddt_In.Number_Piece
                            };
                            _subBatchService.CreateSubBatch(subBatch);
                        }
                    }
                    transaction.Commit();
                    return model.Ddt_In;
                }
                catch (Exception e)
                {
                    /*TODO: Eliminazione Ddt da fatture_in_cloud*/
                    transaction.Rollback();
                    throw;
                }
            }
        }
        
        public Order AddOrderAndSendToFattureInCloud(Order orderToSave)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                var addedOrder = _orderService.AddOrder(orderToSave);
                try
                {
                    var id_fatture = _apiFatturaInCloudService.AddOrderCloud(new OrderDto
                    {
                        Name = addedOrder.Name,
                        Description = addedOrder.Description ?? string.Empty,
                        DataIn = addedOrder.DataIn,
                        DataOut = addedOrder.DataOut,
                        Number_Piece = addedOrder.Number_Piece,
                        Price_Uni = addedOrder.Price_Uni,
                        SKU = addedOrder.SKU,
                        DDT = addedOrder.DDT
                    });

                    addedOrder.ID_FattureInCloud = id_fatture;
                    _orderService.UpdateOrder(addedOrder);

                    transaction.Commit();
                    return addedOrder;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    _orderService.DeleteOrderByID(addedOrder.OrderID);
                    transaction.Commit();
                    throw;
                }
            }
            
        }

        public void RegisterBatchAtCOQ(int subBatchId)
        {
            var subBatch = _subBatchService.GetSubBatchById(subBatchId);
            if (subBatch != null)
            {
                if (subBatch.OperationTimelines == null)
                    subBatch.OperationTimelines = new List<OperationTimeline>();
                var now = DateTime.Now;
                subBatch.OperationTimelines.Add(new OperationTimeline()
                {
                    SubBatchID = subBatchId,
                    BatchOperation = new BatchOperation()
                    {
                        BatchID = subBatch.BatchID,
                        OperationID = _dbContext.Operations.SingleOrDefault(s => s.Name == OtherConstants.COQ).OperationID,
                        Ordering = subBatch.Batch.BatchOperations == null ? 1 : subBatch.Batch.BatchOperations.Max(s => s.Ordering)+1
                    },
                    Status = "A",
                    MachineId = 99,
                    StartDate = now,
                    EndDate = now
                });
            }
            _subBatchService.UpdateSubBatch(subBatch);
        }

        public QualityControlViewModel GetQualityControlViewModel()
        {
            return new QualityControlViewModel()
            {
                OperationTimeLine = _subBatchService.GetSubBatchAtQualityControl()
            };
        }

        public int EndOrder(SubBatchToEndDto dto)
        {
            var subBatch = _subBatchService.GetSubBatchById(dto.SubBatchId);
            if (dto.LostPieces + dto.WastePieces + dto.OkPieces > subBatch.Ddts_In.Sum(s => s.Number_Piece_Now))
                throw new Exception("Il totale dei pezzi inserito è maggiore dei pezzi attualmente in azienda.");
            var ratio = subBatch.Ddts_In.Sum(s => s.Number_Piece_Now);
            var ddts = subBatch.Ddts_In.Where(s => s.Number_Piece_Now > 0).OrderByDescending(s => s.TotalPriority)
                .ToList();
            var lastElement = ddts.LastOrDefault();
            var dtoCopy = dto;
            foreach (var item in ddts)
            {
                if (lastElement != item)
                {
                    var lost = Convert.ToInt32(dto.LostPieces * (Convert.ToDouble(item.Number_Piece) / ratio));
                    var missing = Convert.ToInt32(dto.WastePieces * (Convert.ToDouble(item.Number_Piece) / ratio));
                    var waste = Convert.ToInt32(dto.MissingPieces * (Convert.ToDouble(item.Number_Piece) / ratio));
                    item.NumberLostPiece += lost;
                    dtoCopy.LostPieces -= lost;
                    item.NumberMissingPiece += missing;
                    dtoCopy.MissingPieces -= missing;
                    item.NumberWastePiece += waste;
                    dtoCopy.WastePieces -= waste;
                    item.Number_Piece_Now -= lost + missing + waste;
                }
                else
                {
                    item.NumberLostPiece += dtoCopy.LostPieces;
                    item.NumberMissingPiece += dtoCopy.MissingPieces;
                    item.NumberWastePiece += dtoCopy.WastePieces;
                    item.Number_Piece_Now -= dtoCopy.LostPieces + dtoCopy.MissingPieces + dtoCopy.WastePieces;
                }
            }

            var ddt_out_id = 0;
            var now = DateTime.Now;
            var ddts_out = _orderService.GetDdtOutsByClientIdAndStatus(subBatch.Ddts_In[0].Product.ClientID, DDTOutStatus.STATUS_PENDING);
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
            foreach (var item in subBatch.Ddts_In.OrderByDescending(s => s.TotalPriority).ToList())
            {
                if (item.Number_Piece_Now >= dto.OkPieces)
                {
                    item.Number_Piece_Now -= dto.OkPieces;
                    item.Status = OrderStatusConstants.STATUS_PARTIALLY_COMPLETED;
                    item.Ddt_Associations ??= new List<Ddt_Association>();
                    item.Ddt_Associations.Add(new Ddt_Association()
                    {
                        Date = now,
                        Ddt_In_ID = item.Ddt_In_ID,
                        Ddt_Out_ID = ddt_out_id,
                        NumberPieces = dto.OkPieces
                    });
                    dto.OkPieces = 0;
                    break;
                }

                dto.OkPieces -= item.Number_Piece_Now;
                item.Status = OrderStatusConstants.STATUS_COMPLETED;
                item.DataEnd = now;
                item.Ddt_Associations ??= new List<Ddt_Association>();
                item.Ddt_Associations.Add(new Ddt_Association()
                {
                    Date = now,
                    Ddt_In_ID = item.Ddt_In_ID,
                    Ddt_Out_ID = ddt_out_id,
                    NumberPieces = item.Number_Piece_Now
                });
                item.Number_Piece_Now = 0;
            }

            if (subBatch.Ddts_In.All(s => s.Number_Piece_Now == 0))
            {
                subBatch.Status = OrderStatusConstants.STATUS_COMPLETED;
                foreach (var item in subBatch.OperationTimelines.Where(s => s.Status != OperationTimelineConstant.STATUS_COMPLETED).ToList())
                {
                    
                    if (item.MachineId == 99 && item.OperationTimelineID == dto.OperationTimeLineId)
                    {
                        item.EndDate = DateTime.Now;
                        item.UseForStatics = true;
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
            return subBatch.SubBatchID;
        }

        public List<Ddt_Out> GetDdtsInDelivery()
        {
            return _orderService.GetDdtOutsByStatus(DDTOutStatus.STATUS_PENDING);
        }

        public string EmitDDT(int id)
        {
            
            var ddtOut = _orderService.GetDdtOutById(id);
            var result = _apiFatturaInCloudService.CreateDdtInCloud(ddtOut);
            ddtOut.Status = DDTOutStatus.STATUS_EMITTED;
            ddtOut.FC_Ddt_Out_ID = result.Item2;
            ddtOut.Url = result.Item1;
            try
            {
                _orderService.UpdateDdtOut(ddtOut);

            }
            catch (Exception e)
            {
                _apiFatturaInCloudService.DeleteDdtInCloudById(result.Item2);
            }
            return result.Item1;
        }

        public DDTEmittedViewModel GetDDTEmittedViewModel()
        {
            return new DDTEmittedViewModel()
            {
                DdtOuts = _orderService.GetDdtOutsByStatus(DDTOutStatus.STATUS_EMITTED)
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
                    ddt.Number_Piece = model.Ddt_In.Number_Piece;
                    ddt.NumberMissingPiece = model.Ddt_In.NumberMissingPiece;
                    ddt.Price_Uni = model.Ddt_In.Price_Uni;
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
    }
}