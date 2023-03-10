using Microsoft.EntityFrameworkCore;
using NLog;
using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Data;

namespace RemaSoftware.Domain.Services.Impl
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

        public List<Order> GetChunkedOrders(int skip, int take)
        {
            var query = _dbContext.Orders
                .Include(i => i.Client)
                .OrderByDescending(ob=>ob.DataOut)
                .AsQueryable();
            if (skip > 0)
                query = query.Skip(skip);
            if (take > 0)
                query = query.Take(take);
            return query.ToList();
        }

        public int GetTotalOrdersCount()
        {
            return _dbContext.Orders.Count();
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
            catch(Exception ex)
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

        public IEnumerable<Order> GetOrdersNotCompleted()
        {
            return _dbContext.Orders
                .Include(i=>i.Client)
                .Where(w => w.Status != OrderStatusConstants.STATUS_COMPLETED).AsEnumerable();
        }

        public List<Order> GetOrdersCompleted()
        {
            return _dbContext.Orders
                .Include(i => i.Client)
                .Where(w => w.Status == OrderStatusConstants.STATUS_COMPLETED).ToList();
        }

        public List<Operation> GetOperationsByOrderId(int orderId)
        {
            return _dbContext.Order_Operations.Where(w => w.OrderID == orderId)
                .OrderBy(ob=>ob.Ordering)
                .Select(s=>s.Operations).ToList();
        }

        public Batch GetBatchById(int batchId)
        {
            return _dbContext.Batches
                .Include(b => b.BatchOperations)
                .ThenInclude(o => o.Operations)
                .Include(s => s.SubBatches)
                .ThenInclude(s => s.Ddts_In)
                .SingleOrDefault(s => s.BatchId == batchId);
        }

        public Batch GetBatchByProductIdAndOperationList(int productId, List<int> operationId)
        {
            try
            {
                //TODO: Controllare!!;
                var batchesIEnum = _dbContext.Batches
                    .Include(s => s.SubBatches)
                    .ThenInclude(s => s.Ddts_In)
                    .ThenInclude(s => s.Product)
                    .Include(b => b.BatchOperations).ToList();
                var batches = batchesIEnum.Where(s => s.SubBatches[0].Ddts_In[0].ProductID == productId)
                    .ToList();
                foreach (var item in batches)
                {
                    var opList = new List<int>();
                    foreach (var op in item.BatchOperations)
                    {
                        opList.Add(op.OperationID);
                    }

                    if (opList.All(operationId.Contains) && opList.Count == operationId.Count)
                    {
                        return item;
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Batch CreateBatch(Batch batch)
        {
            try
            {
                var addedBatch = _dbContext.Add(batch);
                _dbContext.SaveChanges();

                return addedBatch.Entity;
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Errore durante la creazione della commessa: {batch.SubBatches[0].Ddts_In[0].Code}");
                return null;
            }
            
        }

        public Ddt_In CreateDDtIn(Ddt_In ddt_In)
        {
            try
            {
                var addedDdtIn = _dbContext.Add(ddt_In);
                _dbContext.SaveChanges();

                return addedDdtIn.Entity;
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Errore durante la creazione della commessa: {ddt_In.Code}");
                return null;
            }
        }

        public List<Ddt_In> GetAllDdtIn()
        {
            return _dbContext.Ddts_In
                .Include(d => d.Product)
                .ThenInclude(s => s.Client)
                .Include(b => b.SubBatch)
                .ThenInclude(s => s.Batch)
                .ThenInclude(b => b.BatchOperations)
                .ToList();
        }

        public Ddt_In GetDdtInById(int id)
        {
            return _dbContext.Ddts_In
                .Include(d => d.Product)
                .ThenInclude(s => s.Client)
                .SingleOrDefault(s => s.Ddt_In_ID == id);
        }

        public List<Ddt_Out> GetDdtOutsByClientIdAndStatus(int id, string status)
        {
            return _dbContext.Ddts_Out
                .Where(s => s.ClientID == id && s.Status == status).ToList();
        }
    }
}