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
                        Description = addedOrder.Description,
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
                    throw;
                }
            }
            
        }
    }
}