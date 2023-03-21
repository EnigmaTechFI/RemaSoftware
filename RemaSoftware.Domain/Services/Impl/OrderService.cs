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
            return _dbContext.Ddts_In.Count(ord => ord.DataOut > DateTime.Now);
        }

        public decimal GetLastMonthEarnings()
        {
            var a = _dbContext.Ddts_In
                .Include(s => s.SubBatch)
                .ThenInclude(s => s.Batch)
                .Where(w => w.DataIn.Month == DateTime.Now.Month && w.IsReso == false)
                .Sum(ord => ord.Price_Uni * ord.Number_Piece);
            return (decimal) a;
        }

        public List<Ddt_In> GetOrdersNearToDeadlineTakeTop(int topSelector)
        {
            return _dbContext.Ddts_In
                .Include(s => s.Product)
                .ThenInclude(i=>i.Client)
                .Where(w=>w.DataOut >= DateTime.Now.Date).OrderBy(ob=>ob.DataOut)
                .Take(topSelector)
                .ToList();
        }
        
        public List<Ddt_In> GetAllOrdersNearToDeadline()
        {
            return _dbContext.Ddts_In
                .Include(s => s.Product)
                .ThenInclude(i=>i.Client)
                .Where(w=>w.DataOut > DateTime.Now.Date).OrderBy(ob=>ob.DataOut)
                .ToList();
        }

        public Order GetOrderBySKU(string sku)
        {
            return _dbContext.Orders.Include(c => c.Client).Where(s => s.SKU == sku).FirstOrDefault();
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
        
        public Ddt_In UpdateDDtIn(Ddt_In ddt_In)
        {
            try
            {
                var addedDdtIn = _dbContext.Update(ddt_In);
                _dbContext.SaveChanges();
                return addedDdtIn.Entity;
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Errore durante l'aggiornamento della commessa: {ddt_In.Code}");
                throw e;
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

        public List<Ddt_In> GetDdtInActive()
        {
            return _dbContext.Ddts_In
                .Include(d => d.Product)
                .ThenInclude(s => s.Client)
                .Include(b => b.SubBatch)
                .ThenInclude(s => s.Batch)
                .ThenInclude(b => b.BatchOperations)
                .Where(s => s.Status == OrderStatusConstants.STATUS_ARRIVED || s.Status == OrderStatusConstants.STATUS_WORKING || s.Status == OrderStatusConstants.STATUS_PARTIALLY_COMPLETED)
                .ToList();
        }

        public List<Ddt_In> GetDdtInWorkingByClientId(int Id)
        {
            return _dbContext.Ddts_In
                .Include(d => d.Product)
                .ThenInclude(s => s.Client)
                .Include(b => b.SubBatch)
                .ThenInclude(s => s.Batch)
                .ThenInclude(b => b.BatchOperations)
                .Where(s => s.Status == OrderStatusConstants.STATUS_WORKING && s.Product.ClientID == Id)
                .ToList();
        }

        public List<Ddt_In> GetDdtInEnded()
        {
            return _dbContext.Ddts_In
                .Include(d => d.Product)
                .ThenInclude(s => s.Client)
                .Include(b => b.SubBatch)
                .ThenInclude(s => s.Batch)
                .ThenInclude(b => b.BatchOperations)
                .Where(s => s.Status == OrderStatusConstants.STATUS_COMPLETED)
                .ToList();
        }

        public List<Ddt_In> GetDdtInStockByClientId(int Id)
        {
            return _dbContext.Ddts_In
                .Include(d => d.Product)
                .ThenInclude(s => s.Client)
                .Include(b => b.SubBatch)
                .ThenInclude(s => s.Batch)
                .ThenInclude(b => b.BatchOperations)
                .Where(s => s.Status == OrderStatusConstants.STATUS_ARRIVED && s.Product.ClientID == Id)
                .ToList();
        }

        public Ddt_In GetDdtInById(int id)
        {
            return _dbContext.Ddts_In
                .Include(d => d.Product)
                .ThenInclude(s => s.Client)
                .Include(s => s.SubBatch)
                .SingleOrDefault(s => s.Ddt_In_ID == id);
        }

        public List<Ddt_Out> GetDdtOutsByClientIdAndStatus(int id, string status)
        {
            return _dbContext.Ddts_Out
                .Include(s => s.Ddt_Associations)
                .Where(s => s.ClientID == id && s.Status == status).ToList();
        }

        public Ddt_Out CreateNewDdtOut(Ddt_Out ddtOut)
        {
            _dbContext.Ddts_Out.Add(ddtOut);
            _dbContext.SaveChanges();
            return ddtOut;
        }

        public List<Ddt_Out> GetDdtOutsByStatus(string status)
        {
            return _dbContext.Ddts_Out
                .Where(s =>s.Status == status)
                .Include(s => s.Ddt_Associations)
                .ThenInclude(s => s.Ddt_In)
                .ThenInclude(s => s.Product)
                .ThenInclude(s => s.Client)
                .ToList();
        }

        public Ddt_Out GetDdtOutById(int id)
        {
            return _dbContext.Ddts_Out
                .Include(s => s.Ddt_Associations)
                .ThenInclude(s => s.Ddt_In)
                .ThenInclude(s => s.SubBatch)
                .ThenInclude(s => s.Batch)
                .Include(s => s.Ddt_Associations)
                .ThenInclude(s => s.Ddt_In)
                .ThenInclude(s => s.Product)
                .ThenInclude(s => s.Client)
                .SingleOrDefault(s => s.Ddt_Out_ID == id);
        }

        public void UpdateDdtOut(Ddt_Out ddt)
        {
            _dbContext.Ddts_Out.Update(ddt);
            _dbContext.SaveChanges();
        }

        public Ddt_Out GetDdtOutsById(int id)
        {
            return _dbContext.Ddts_Out.SingleOrDefault(s => s.Ddt_Out_ID == id);
        }

        public Ddt_Out CreateDDTOut(Ddt_Out ddtOut)
        {
            _dbContext.Ddts_Out.Add(ddtOut);
            _dbContext.SaveChanges();
            return ddtOut;
        }

        public void UpdateDdtAssociationByIdWithNewDdtOut(int ddtAssociationId, int ddtOutDdtOutId)
        {
            var ddt = _dbContext.Ddt_Associations.SingleOrDefault(s => s.ID == ddtAssociationId);
            ddt.Ddt_Out_ID = ddtOutDdtOutId;
            _dbContext.Ddt_Associations.Update(ddt);
            _dbContext.SaveChanges();
        }

        public void DeleteDDT(Ddt_In ddt)
        {
            _dbContext.Ddts_In.Remove(ddt);
            _dbContext.SaveChanges();
        }

        public void DeleteSubBatch(SubBatch subBatch)
        {
            _dbContext.SubBatches.Remove(subBatch);
            _dbContext.SaveChanges();
        }

        public void DeleteBatch(Batch batch)
        {
            _dbContext.Batches.Remove(batch);
            _dbContext.SaveChanges();
        }
    }
}