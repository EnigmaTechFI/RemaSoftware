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

        public int GetTotalProcessedPiecese()
        {
            return _dbContext.Ddts_In
                .Where(s => s.Status != OrderStatusConstants.STATUS_COMPLETED && s.Status != OrderStatusConstants.STATUS_DELIVERED)
                .Sum(s => s.Number_Piece);
        }

        public int GetCountOrdersNotExtinguished()
        {
            return _dbContext.Ddts_In.Count(s => s.Status != OrderStatusConstants.STATUS_COMPLETED && s.Status != OrderStatusConstants.STATUS_DELIVERED);
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
                .Where(w=> w.Status != OrderStatusConstants.STATUS_COMPLETED && w.Status != OrderStatusConstants.STATUS_DELIVERED).OrderBy(ob=>ob.DataOut)
                .Take(topSelector)
                .ToList();
        }
        
        public List<Ddt_In> GetAllOrdersNearToDeadline()
        {
            return _dbContext.Ddts_In
                .Include(s => s.Product)
                .ThenInclude(i=>i.Client)
                .Where(w=> w.Status != OrderStatusConstants.STATUS_COMPLETED && w.Status != OrderStatusConstants.STATUS_DELIVERED).OrderBy(ob=>ob.DataOut)
                .ToList();
        }

        public Batch GetBatchById(int batchId)
        {
            return _dbContext.Batches
                .Include(b => b.BatchOperations)
                .ThenInclude(o => o.Operations)
                .Include(s => s.SubBatches)
                .ThenInclude(s => s.OperationTimelines)
                .Include(s => s.SubBatches)
                .ThenInclude(s => s.Ddts_In)
                .ThenInclude(s => s.Product)
                .ThenInclude(s => s.Client)
                .SingleOrDefault(s => s.BatchId == batchId);
        }

        public Batch GetBatchByProductIdAndOperationList(int productId, List<int> operationId)
        {
            try
            {
                var batchesIEnum = _dbContext.Batches
                    .Include(s => s.SubBatches)
                    .ThenInclude(s => s.Ddts_In)
                    .ThenInclude(s => s.Product)
                    .Include(b => b.BatchOperations)
                    .Where(s => s.SubBatches.Count != 0)
                    .ToList();
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
                .ThenInclude(s => s.Batch)
                .ThenInclude(s => s.BatchOperations)
                .ThenInclude(s => s.Operations)
                .Include(s => s.Ddt_Associations)
                .ThenInclude(s => s.Ddt_Out)
                .SingleOrDefault(s => s.Ddt_In_ID == id);
        }

        public List<Ddt_Out> GetDdtOutsByClientIdAndStatus(int id, string status)
        {
            return _dbContext.Ddts_Out
                .Include(s => s.Ddt_Associations)
                .ThenInclude(s => s.Ddt_In)
                .ThenInclude(s => s.Product)
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
            _dbContext.BatchOperations.RemoveRange(batch.BatchOperations);
            _dbContext.Batches.Remove(batch);
            _dbContext.SaveChanges();
        }

        public List<Ddt_In> GetDdtInActiveByClientId(int clientId)
        {
            return _dbContext.Ddts_In
                .Include(d => d.Product)
                .ThenInclude(s => s.Client)
                .Include(b => b.SubBatch)
                .ThenInclude(s => s.Batch)
                .ThenInclude(b => b.BatchOperations)
                .Where(s => s.Product.ClientID == clientId && (s.Status == OrderStatusConstants.STATUS_ARRIVED || s.Status == OrderStatusConstants.STATUS_WORKING || s.Status == OrderStatusConstants.STATUS_PARTIALLY_COMPLETED))
                .ToList();
        }

        public List<Ddt_In> GetDdtInEndedByClientId(int clientId)
        {
            return _dbContext.Ddts_In
                .Include(d => d.Product)
                .ThenInclude(s => s.Client)
                .Include(b => b.SubBatch)
                .ThenInclude(s => s.Batch)
                .ThenInclude(b => b.BatchOperations)
                .Where(s => s.Product.ClientID == clientId && s.Status == OrderStatusConstants.STATUS_COMPLETED)
                .ToList();
        }

        public void CreateDDTAssociation(Ddt_Association ass)
        {
            _dbContext.Ddt_Associations.Add(ass);
            _dbContext.SaveChanges();
        }

        public List<Label> GetLastLabelOut()
        {
            return _dbContext.Label.OrderByDescending(s => s.Date).Take(10).ToList();
        }

        public void CreateNewLabelOut(Label label)
        {
            _dbContext.Label.Add(label);
            _dbContext.SaveChanges();
        }

        public List<Batch> GetAllBatch()
        {
            return _dbContext.Batches
                .Include(s => s.SubBatches)
                .ThenInclude(s => s.Ddts_In)
                .ThenInclude(s => s.Product)
                .ThenInclude(s => s.Client)
                .Where(s => s.SubBatches.Count > 0)
                .ToList();
        }

        public Ddt_Association GetDDTAssociationById(int id)
        {
            return _dbContext.Ddt_Associations
                .Include(s => s.Ddt_In)
                .ThenInclude(s => s.SubBatch)
                .Include(s => s.Ddt_Out)
                .ThenInclude(s => s.Ddt_Associations)
                .SingleOrDefault(s => s.ID == id);

        }

        public void DeleteDDTAssociation(Ddt_Association ddtAssociation)
        {
            _dbContext.Ddt_Associations.Remove(ddtAssociation);
            _dbContext.SaveChanges();
        }

        public void DeleteDDTOut(Ddt_Out ddtOut)
        {
            _dbContext.Ddts_Out.Remove(ddtOut);
            _dbContext.SaveChanges();
        }

        public Ddt_Supplier CreateNewDdtSupplier(Ddt_Supplier modelDdtSupplier)
        {
            _dbContext.Ddt_Suppliers.Add(modelDdtSupplier);
            _dbContext.SaveChanges();
            return modelDdtSupplier;
        }

        public void CreateNewDdtSupplierAssociation(DDT_Supplier_Association entity)
        {
            _dbContext.DdtSupplierAssociations.Add(entity);
            _dbContext.SaveChanges();
        }

        public void CreateNewDdtSuppliersAssociation(List<DDT_Supplier_Association> ddtSupplierAssociations)
        {
            _dbContext.DdtSupplierAssociations.AddRange(ddtSupplierAssociations);
            _dbContext.SaveChanges();
        }

        public void UpdateDDtSupplier(Ddt_Supplier ddtSupplier)
        {
            _dbContext.Ddt_Suppliers.Update(ddtSupplier);
            _dbContext.SaveChanges();
        }

        public Ddt_Supplier GetDdtSupplierById(int modelDdtSupplierId)
        {
            return _dbContext.Ddt_Suppliers
                .Include(s => s.DdtSupplierAssociations)
                .ThenInclude(s => s.Ddt_In)
                .ThenInclude(s => s.Product)
                .ThenInclude(s => s.Client)
                .Include(s => s.OperationTimeline)
                .SingleOrDefault(s => s.Ddt_Supplier_ID == modelDdtSupplierId);
        }

        public void UpdateDDtInRange(List<Ddt_In> ddts)
        {
            _dbContext.Ddts_In.UpdateRange(ddts);
            _dbContext.SaveChanges();
        }
    }
}