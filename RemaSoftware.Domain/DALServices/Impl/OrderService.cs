using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NLog;
using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.ContextModels;
using RemaSoftware.Domain.Data;

namespace RemaSoftware.Domain.DALServices.Impl
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

        public Order GetOrderWithOperationsById(int orderId)
        {
            var order = _dbContext.Orders
                .Include(i=>i.Client)
                .Include(i=>i.Order_Operation)
                .ThenInclude(ti=>ti.Operations)
                .SingleOrDefault(sd => sd.OrderID == orderId);
            
            if (order == null)
                throw new Exception($"Order not found with Id: {orderId}");
            
            order.Order_Operation = order.Order_Operation.OrderBy(ob => ob.Ordering).ToList();
            return order;
        }

        public Order AddOrder(Order order)
        {
            if(order == null)
                throw new Exception("Ordine vuoto.");
            try
            {
                var addedOrder = _dbContext.Add(order);
                _dbContext.SaveChanges();

                return addedOrder.Entity;
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Errore durante l'aggiunta dell'ordine: {order.ToString()}");
            }
            return null;
        }

        public List<Order> GetOrdersByFilters()
        {
            throw new System.NotImplementedException();
        }
        
        public void AddOrderOperation(int orderId, List<int> operationId)
        {
            var Order_Op = new List<Order_Operation>();

            var counterOrdering = 1;
            foreach(var id in operationId)
            {
                Order_Operation op = new Order_Operation
                {
                    OrderID = orderId,
                    OperationID = id,
                    Ordering = counterOrdering
                };
                Order_Op.Add(op);
                counterOrdering++;
            }

            _dbContext.AddRange(Order_Op);
            _dbContext.SaveChanges();
        }

        public int GetTotalProcessedPiecese()
        {
            return _dbContext.Orders.Where(w => w.DataOut < DateTime.Now).Sum(s => s.Number_Piece);
        }

        public int GetCountOrdersNotExtinguished()
        {
            return _dbContext.Orders.Count(ord => ord.DataOut > DateTime.Now);
        }

        public decimal GetLastMonthEarnings()
        {
            var a = _dbContext.Orders.Where(w => w.DataOut.Month == DateTime.Now.Month)
                .Sum(ord => ord.Price_Uni * ord.Number_Piece);
            return (decimal) a;
        }

        public List<Order> GetOrdersNearToDeadlineTakeTop(int topSelector)
        {
            return _dbContext.Orders
                .Include(i=>i.Client)
                .Where(w=>w.DataOut >= DateTime.Now.Date).OrderBy(ob=>ob.DataOut)
                .Take(topSelector)
                .ToList();
        }
        
        public List<Order> GetAllOrdersNearToDeadline()
        {
            return _dbContext.Orders
                .Include(i=>i.Client)
                .Where(w=>w.DataOut > DateTime.Now.Date).OrderBy(ob=>ob.DataOut)
                .ToList();
        }

        public List<string> GetOldOrders_SKU()
        {
            return _dbContext.Orders.Select(s => s.SKU).ToList();
        }

        public Order GetOrderBySKU(string sku)
        {
            return _dbContext.Orders.Include(c => c.Client).Where(s => s.SKU == sku).FirstOrDefault();
        }

        public bool UpdateOrder(Order order)
        {
            try
            {
                _dbContext.Orders.Update(order);
                _dbContext.SaveChanges();
                return true;

            }
            catch
            {
                return false;
            }
        }

        public bool DeleteOrderByID(int OrderID)
        {
            try
            {
                var order = _dbContext.Orders.SingleOrDefault(i => i.OrderID == OrderID);
                _dbContext.Remove(order);
                _dbContext.SaveChanges();
                return true;
            }
            catch
            {
                Logger.Error($"[Eliminazione Ordine] Ordine non presente. ID: {OrderID}");
                return false;
            }
            
        }

        public void UpdateOrderStatus(int orderId, int outgoing_orders)
        {
            var order = _dbContext.Orders.SingleOrDefault(sd => sd.OrderID == orderId);
            if (order == null)
                throw new Exception($"Ordine non trovato con id: {orderId}.");

            order.Number_Pieces_InStock = order.Number_Pieces_InStock - outgoing_orders;

            if (order.Number_Pieces_InStock < 0)
                throw new Exception($"Numero pezzi attuale minore rispetto a numero pezzi in uscita ({outgoing_orders})");

            else if(order.Number_Pieces_InStock == 0 )
            {
                order.Status = "C";
            }

            else if(order.Number_Pieces_InStock > 0)
            {
                order.Status = "B";
            }

            _dbContext.Orders.Update(order);
            _dbContext.SaveChanges();
        }

        public List<Order> GetOrdersNotCompleted()
        {
            return _dbContext.Orders
                .Include(i=>i.Client)
                .Where(w => w.Status != OrderStatusConstants.STATUS_COMPLETED).ToList();
        }

        public List<Operation> GetOperationsByOrderId(int orderId)
        {
            return _dbContext.Order_Operations.Where(w => w.OrderID == orderId)
                .OrderBy(ob=>ob.Ordering)
                .Select(s=>s.Operations).ToList();
        }
    }
}