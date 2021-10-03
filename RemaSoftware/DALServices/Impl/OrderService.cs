using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NLog;
using RemaSoftware.ContextModels;
using RemaSoftware.Data;

namespace RemaSoftware.DALServices.Impl
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _dbContext;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public OrderService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public List<Order> GetAllOrders()
        {
            return _dbContext.Orders
                .Include(i=>i.Client).ToList();
        }

        public Order GetOrderById(int orderId)
        {
            return _dbContext.Orders.Include(i=>i.Client).SingleOrDefault(sd => sd.OrderID == orderId);
        }

        public void AddOrder(Order order)
        {
            if(order == null)
                throw new Exception("Ordine vuoto.");
            try
            {
                _dbContext.Add(order);
                _dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Errore durante l'aggiunta dell'ordine: {order.ToString()}");
            }
        }

        public List<Order> GetOrdersByFilters()
        {
            throw new System.NotImplementedException();
        }
    }
}