using Microsoft.EntityFrameworkCore;
using NLog;
using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.Data;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.Domain.Services.Impl;

public class SubBatchService : ISubBatchService
{
    private readonly ApplicationDbContext _dbContext;
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public SubBatchService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void UpdateSubBatch(SubBatch entity)
    {
        try
        {
            _dbContext.SubBatches.Update(entity);
            _dbContext.SaveChanges();
        }
        catch (Exception e)
        {
            Logger.Error(e, e.Message);
            throw new Exception("Errore SQL aggiornamento subBatch.");
        }
    }

    public void UpdateSubBatchStatus(int Id, string status)
    {
        try
        {
            var sb =_dbContext.SubBatches
                .Include(s =>s.Ddts_In)
                .SingleOrDefault(s => s.SubBatchID == Id);
            sb.Status = status;
            sb.Ddts_In = sb.Ddts_In.Select(x =>
            {
                x.Status = status;
                return x;
            }).ToList();
            _dbContext.SubBatches.Update(sb);
            _dbContext.SaveChanges();
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public async Task<List<OperationTimeline>> UpdateSubBatchStatusAndOperationTimelineStart(int Id, int machineId, int batchOperationId, int numbersOperator, DateTime start)
    {
        using (var transaction = _dbContext.Database.BeginTransaction())
        {
            try
            {
                var sb = _dbContext.SubBatches
                    .Include(s => s.Ddts_In)
                    .ThenInclude(s => s.Product)
                    .ThenInclude(s => s.Client)
                    .Include(s => s.OperationTimelines)
                    .ThenInclude(s => s.BatchOperation)
                    .ThenInclude(s => s.Operations)
                    .SingleOrDefault(s => s.SubBatchID == Id);
                if (sb.Status == OrderStatusConstants.STATUS_COMPLETED ||
                    sb.Status == OrderStatusConstants.STATUS_DELIVERED)
                    throw new Exception("Ordine già concluso.");
                if (sb == null)
                    throw new Exception("Ordine non presente");
                sb.Status = OrderStatusConstants.STATUS_WORKING;
                var op = new List<OperationTimeline>();
                for (int i = 0; i < numbersOperator; i++)
                {
                    op.Add(new OperationTimeline()
                    {
                        BatchOperation = _dbContext.BatchOperations.Include(s => s.Operations).SingleOrDefault(s => s.BatchOperationID == batchOperationId),
                        BatchOperationID = batchOperationId,
                        StartDate = start,
                        EndDate = start,
                        MachineId = machineId,
                        Status = OperationTimelineConstant.STATUS_WORKING,
                        SubBatch = sb,
                        SubBatchID = sb.SubBatchID
                    });
                }

                _dbContext.OperationTimelines.AddRange(op);
                sb.Ddts_In.Where(s => s.Status == OrderStatusConstants.STATUS_ARRIVED).ToList().ForEach(s => s.Status = OrderStatusConstants.STATUS_WORKING);

                _dbContext.Update(sb);
                _dbContext.SaveChanges();
                transaction.Commit();
                return op;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                Console.WriteLine(e);
                throw e;
            }
        }
    }

    public OperationTimeline UpdateSubBatchStatusAndOperationTimelineEnd(int operationTimelineId, DateTime end, string status)
    {
        var op =_dbContext.OperationTimelines
            .Include(s =>s.SubBatch)
            .ThenInclude(s => s.Ddts_In)
            .SingleOrDefault(s => s.OperationTimelineID == operationTimelineId);
        if (op.Status == OperationTimelineConstant.STATUS_COMPLETED)
            return op;
        
        if (op.Status == OperationTimelineConstant.STATUS_PAUSE)
        {
            op.Status = status;
            _dbContext.OperationTimelines.Update(op);
            _dbContext.SaveChanges();
            return op;
        }
            
        if (op != null)
        {
            var controlTime = end.DayOfYear - op.StartDate.DayOfYear;
            if ( controlTime> 0)
            {
                op.Status = OperationTimelineConstant.STATUS_COMPLETED;
                op.EndDate = new DateTime(op.StartDate.Year, op.StartDate.Month, op.StartDate.Day, 17, 0, 0);
                op.UseForStatics = true;
                var dateRif = op.StartDate;
                for (int i = 1; i <= controlTime; i++)
                {
                    dateRif = dateRif.AddDays(1);
                    if (dateRif.DayOfWeek != DayOfWeek.Sunday && dateRif.DayOfWeek != DayOfWeek.Saturday)
                    {
                        _dbContext.OperationTimelines.Add(new OperationTimeline()
                        {
                            BatchOperationID = op.BatchOperationID,
                            EndDate = i == controlTime
                                ? end
                                : new DateTime(dateRif.Year, dateRif.Month, dateRif.Day, 17, 30, 0),
                            StartDate = new DateTime(dateRif.Year, dateRif.Month, dateRif.Day, 7, 30, 0),
                            Status = i == controlTime ? status : OperationTimelineConstant.STATUS_COMPLETED,
                            UseForStatics = true,
                            MachineId = op.MachineId,
                            SubBatchID = op.SubBatchID
                        });
                    }
                    else if (dateRif.DayOfWeek == DayOfWeek.Saturday)
                    {
                        _dbContext.OperationTimelines.Add(new OperationTimeline()
                        {
                            BatchOperationID = op.BatchOperationID,
                            EndDate = i == controlTime
                                ? end
                                : new DateTime(dateRif.Year, dateRif.Month, dateRif.Day, 12, 0, 0),
                            StartDate = new DateTime(dateRif.Year, dateRif.Month, dateRif.Day, 8, 0, 0),
                            Status = i == controlTime ? status : OperationTimelineConstant.STATUS_COMPLETED,
                            UseForStatics = true,
                            MachineId = op.MachineId,
                            SubBatchID = op.SubBatchID
                        });
                    }
                }
            }
            else
            {
                op.Status = status;
                op.UseForStatics = true;
                op.EndDate = end;    
            }
            _dbContext.OperationTimelines.Update(op);
            _dbContext.SaveChanges();
            return op;
        }

        throw new Exception("Nessuna operazione trovata.");
    }

    public void CreateSubBatch(SubBatch entity)
    {
        try
        {
            _dbContext.SubBatches.Add(entity);
            _dbContext.SaveChanges();
        }
        catch (Exception e)
        {
            Logger.Error(e, e.Message);
            throw new Exception("Errore SQL creazione subBatch.");
        }
    }

    public List<SubBatch> GetSubBatchesStatus(string status)
    {
        return _dbContext.SubBatches
            .Where(s => s.Status == status)
            .Include(s => s.Ddts_In)
            .ThenInclude(s => s.Product)
            .ThenInclude(s => s.Client)
            .ToList();
    }

    public List<SubBatch> GetSubBatchesStatusForOrderSummary(string status)
    {
        return _dbContext.SubBatches
            .Where(s => s.Status == status)
            .Include(s => s.Ddts_In)
            .ThenInclude(s => s.Product)
            .ThenInclude(s => s.Client)
            .ToList();
    }

    public SubBatch GetSubBatchById(int id)
    {
        return _dbContext.SubBatches
            .Include(s => s.Batch)
            .ThenInclude(s => s.BatchOperations)
            .ThenInclude(s => s.Operations)
            .Include(s => s.Batch)
            .ThenInclude(s => s.BatchOperations)
            .ThenInclude(s => s.OperationTimelines)
            .ThenInclude(s => s.DdtSupplier)
            .ThenInclude(s => s.Supplier)
            .Include(s => s.Ddts_In)
            .ThenInclude(s => s.Product)
            .ThenInclude(s => s.Client)
            .SingleOrDefault(s => s.SubBatchID == id);
    }
    
    public SubBatch GetSubBatchByIdForMobile(int id)
    {
        return _dbContext.SubBatches
            .Include(s => s.Batch)
            .ThenInclude(s => s.BatchOperations.Where(s => s.Operations.Name != OtherConstants.COQ))
            .ThenInclude(s => s.Operations)
            .Include(s => s.Ddts_In)
            .ThenInclude(s => s.Product)
            .ThenInclude(s => s.Client)
            .SingleOrDefault(s => s.SubBatchID == id);
    }

    public List<OperationTimeline> GetOperationTimelinesByStatus(string status)
    {
        return _dbContext.OperationTimelines
            .Include(s => s.BatchOperation)
            .ThenInclude(s => s.Operations)
            .Include(s => s.SubBatch)
            .ThenInclude(s => s.Batch)
            .Include(s => s.SubBatch)
            .ThenInclude(s => s.Ddts_In)
            .ThenInclude(s => s.Product)
            .ThenInclude(s => s.Client)
            .Where(s => s.Status == status)
            .ToList();
    }
    
    public List<OperationTimeline> GetOperationTimelinesByMulitpleStatus(List<string> status)
    {
        return _dbContext.OperationTimelines
            .Include(s => s.BatchOperation)
            .ThenInclude(s => s.Operations)
            .Include(s => s.SubBatch)
            .ThenInclude(s => s.Batch)
            .Include(s => s.SubBatch)
            .ThenInclude(s => s.Ddts_In)
            .ThenInclude(s => s.Product)
            .ThenInclude(s => s.Client)
            .Where(s => status.Contains(s.Status))
            .ToList();
    }

    public List<OperationTimeline> GetOperationTimelinesForMachine(int machineId)
    {
        return _dbContext.OperationTimelines
            .Include(s => s.BatchOperation)
            .ThenInclude(s => s.Operations)
            .Include(s => s.SubBatch)
            .ThenInclude(s => s.Ddts_In)
            .ThenInclude(s => s.Product)
            .ThenInclude(s => s.Client)
            .Where(s => s.MachineId == machineId && (s.Status == OperationTimelineConstant.STATUS_WORKING || s.Status == OperationTimelineConstant.STATUS_PAUSE))
            .ToList();
    }

    public List<SubBatch> GetSubBatchesStatusAndClientId(string status, int clientId)
    {
        return _dbContext.SubBatches
            .Include(s => s.Batch)
            .ThenInclude(s => s.BatchOperations)
            .ThenInclude(s => s.Operations)
            .Include(s => s.Ddts_In)
            .ThenInclude(s => s.Product)
            .ThenInclude(s => s.Client)
            .Where(s => s.Status == status && s.Ddts_In.All(s => s.Product.ClientID == clientId))
            .ToList();
    }

    public SubBatch GetSubBatchByIdAndClientId(int id, int clientId)
    {
        return _dbContext.SubBatches
            .Include(s => s.Batch)
            .ThenInclude(s => s.BatchOperations)
            .ThenInclude(s => s.Operations)
            .Include(s => s.Batch)
            .ThenInclude(s => s.BatchOperations)
            .ThenInclude(s => s.OperationTimelines)
            .Include(s => s.Ddts_In)
            .ThenInclude(s => s.Product)
            .ThenInclude(s => s.Client)
            .SingleOrDefault(s => s.SubBatchID == id && s.Ddts_In.Any(s => s.Product.ClientID == clientId));
    }

    public List<SubBatch> GetSubBatchesWithExtras()
    {
        return _dbContext.SubBatches
            .Include(s => s.OperationTimelines)
            .Include(s => s.Batch)
            .ThenInclude(s => s.BatchOperations)
            .ThenInclude(s => s.Operations)
            .Include(s => s.Ddts_In)
            .ThenInclude(s => s.Product)
            .ThenInclude(s => s.Client)
            .Where(s => s.Batch.BatchOperations.Any(s => s.OperationTimelines.Any(s => s.BatchOperation.Operations.Name == OtherConstants.EXTRA)))
            .ToList();
    }

    public List<Ddt_Supplier> GetSubBatchToSupplier()
    {
        return _dbContext.Ddt_Suppliers
            .Include(s => s.Supplier)
            .Include(s => s.DdtSupplierAssociations)
            .ThenInclude(s => s.Ddt_In)
            .ThenInclude(s => s.SubBatch)
            .Include(s => s.DdtSupplierAssociations)
            .ThenInclude(s => s.Ddt_In)
            .ThenInclude(s => s.Product)
            .Where(s => s.Status == OrderStatusConstants.STATUS_WORKING)
            .ToList();
    }

    public List<OperationTimeline> GetSubBatchAtQualityControl()
    {
        return _dbContext.OperationTimelines
            .Include(s => s.BatchOperation)
            .ThenInclude(s => s.Operations)
            .Include(s => s.SubBatch)
            .ThenInclude(s => s.Batch)
            .Include(s => s.SubBatch)
            .ThenInclude(s => s.Ddts_In)
            .ThenInclude(s => s.Product)
            .ThenInclude(s => s.Client)
            .Where(s => s.BatchOperation.Operations.Name == OtherConstants.COQ && s.Status == OperationTimelineConstant.STATUS_WORKING)
            .ToList();
    }
}