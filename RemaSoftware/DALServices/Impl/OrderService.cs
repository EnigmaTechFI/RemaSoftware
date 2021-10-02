using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RemaSoftware.ContextModels;
using RemaSoftware.Data;

namespace RemaSoftware.DALServices.Impl
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _dbContext;

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

            _dbContext.Add(order);
            _dbContext.SaveChanges();
        }

        public List<Order> GetOrdersByFilters()
        {
            throw new System.NotImplementedException();
        }
    }
}