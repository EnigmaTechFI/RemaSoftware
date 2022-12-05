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
        private readonly IAPIFatturaInCloudService _apiFatturaInCloudService;
        private readonly ApplicationDbContext _dbContext;

        public OrderHelper(IOrderService orderService, IAPIFatturaInCloudService apiFatturaInCloudService, ApplicationDbContext dbContext)
        {
            _orderService = orderService;
            _apiFatturaInCloudService = apiFatturaInCloudService;
            _dbContext = dbContext;
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