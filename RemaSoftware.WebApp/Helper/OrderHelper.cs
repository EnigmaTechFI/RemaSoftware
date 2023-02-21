using System;
using System.Drawing;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;
using RemaSoftware.Domain.Data;
using RemaSoftware.UtilityServices;
using RemaSoftware.UtilityServices.Dtos;
using UtilityServices.Dtos;
using RemaSoftware.WebApp.Models.OrderViewModel;
using System.Collections.Generic;
using System.Linq;

namespace RemaSoftware.WebApp.Helper
{
    public class OrderHelper
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IAPIFatturaInCloudService _apiFatturaInCloudService;
        private readonly ApplicationDbContext _dbContext;

        public OrderHelper(IOrderService orderService, IAPIFatturaInCloudService apiFatturaInCloudService, ApplicationDbContext dbContext, IProductService productService)
        {
            _orderService = orderService;
            _apiFatturaInCloudService = apiFatturaInCloudService;
            _dbContext = dbContext;
            _productService = productService;
        }

        public List<Batch> GetBatchByDDTStatus(string status)
        {   
            return _orderService.GetBatchesByDDTStatus(status);
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
                    var ddtList = new List<Ddt_In>();
                    ddtList.Add(model.Ddt_In);
                    if (batch == null)
                    {
                        _orderService.CreateBatch(new Batch()
                        {
                            Ddts_In = ddtList,
                            Price_Uni = model.uni_price.Value,
                            BatchOperations = batchOperationList
                        });
                    }
                    else
                    {
                        model.Ddt_In.BatchID = batch.BatchId;
                        _orderService.CreateDDtIn(model.Ddt_In);
                    }
                    transaction.Commit();
                    return model.Ddt_In;
                }
                catch (Exception e)
                {
                    /*TODO: Eliminazione Ddt da fatture_in_cloud*/
                    transaction.Rollback();
                    transaction.Commit();
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