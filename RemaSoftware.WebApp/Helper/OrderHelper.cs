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
using RemaSoftware.UtilityServices.Interface;

namespace RemaSoftware.WebApp.Helper
{
    public class OrderHelper
    {
        private readonly IOrderService _orderService;
        private readonly ISubBatchService _subBatchService;
        private readonly IProductService _productService;
        private readonly IAPIFatturaInCloudService _apiFatturaInCloudService;
        private readonly ApplicationDbContext _dbContext;

        public OrderHelper(IOrderService orderService, IAPIFatturaInCloudService apiFatturaInCloudService, ApplicationDbContext dbContext, IProductService productService, ISubBatchService subBatchService)
        {
            _orderService = orderService;
            _apiFatturaInCloudService = apiFatturaInCloudService;
            _dbContext = dbContext;
            _productService = productService;
            _subBatchService = subBatchService;
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

        public Ddt_In AddNewDdtIn(NewOrderViewModel model)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var operationsSelected = model.OperationsSelected.Where(w => !w.StartsWith("0"))
                        .Select(s => int.Parse(s.Split('-').First())).ToList();
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
                        _productService.GetProductById(model.Ddt_In.ProductID).SKU, model.uni_price.Value);
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
                            Price_Uni = model.uni_price.Value,
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
                    #if !DEBUG
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
                    #endif
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

        public Order UpdateOrderAndSendToFattureInCloud(Order orderToSave)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                var addedOrder = _orderService.UpdateOrder(orderToSave);
                try
                {
                    var result = _apiFatturaInCloudService.UpdateOrderCloud(new OrderToUpdateDto
                    {
                        Id = orderToSave.ID_FattureInCloud,
                        Name = orderToSave.Name,
                        Description = orderToSave.Description,
                        DataIn = orderToSave.DataIn,
                        DataOut = orderToSave.DataOut,
                        Number_Piece = orderToSave.Number_Piece,
                        Price_Uni = orderToSave.Price_Uni,
                        SKU = orderToSave.SKU,
                        DDT = orderToSave.DDT,
                        ID_FattureInCloud = orderToSave.ID_FattureInCloud
                    });

                    if (!result)
                        throw new Exception();

                    transaction.Commit();
                    return orderToSave;
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