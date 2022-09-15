using System;
using System.Drawing;
using RemaSoftware.ContextModels;
using RemaSoftware.DALServices;
using RemaSoftware.Data;
using UtilityServices;
using UtilityServices.Dtos;

namespace RemaSoftware.Helper
{
    public class OrderHelper
    {
        private readonly IOrderService _orderService;
        private readonly IAPIContabilit‡InCloudService _apiContabilit‡InCloudService;
        private readonly ApplicationDbContext _dbContext;

        public OrderHelper(IOrderService orderService, IAPIContabilit‡InCloudService apiContabilit‡InCloudService, ApplicationDbContext dbContext)
        {
            _orderService = orderService;
            _apiContabilit‡InCloudService = apiContabilit‡InCloudService;
            _dbContext = dbContext;
        }
        
        public Order AddOrderAndSendToFattureInCloud(Order orderToSave)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                var addedOrder = _orderService.AddOrder(orderToSave);
                try
                {
                    _apiContabilit‡InCloudService.AddOrderCloud(new OrderDto
                    {
                        Id = addedOrder.OrderID,
                        Name = addedOrder.Name,
                        Description = addedOrder.Description,
                        DataIn = addedOrder.DataIn,
                        DataOut = addedOrder.DataOut,
                        Number_Piece = addedOrder.Number_Piece,
                        Price_Uni = addedOrder.Price_Uni,
                        SKU = addedOrder.SKU,
                        DDT = addedOrder.DDT
                    });

                    _apiContabilit‡InCloudService.UpdateInventory(new OrderDto
                    {
                        Id = addedOrder.OrderID,
                        Name = addedOrder.Name,
                        Description = addedOrder.Description,
                        DataIn = addedOrder.DataIn,
                        DataOut = addedOrder.DataOut,
                        Number_Piece = addedOrder.Number_Piece,
                        Price_Uni = addedOrder.Price_Uni,
                        SKU = addedOrder.SKU,
                        DDT = addedOrder.DDT
                    });

                    transaction.Commit();
                    return addedOrder;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    _orderService.DeleteOrderByID(addedOrder.OrderID);
                    throw;
                }
            }
            
        }
    }
}