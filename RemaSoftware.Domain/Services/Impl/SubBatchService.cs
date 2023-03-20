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
        try
        {
            var sb = _dbContext.SubBatches
                .Include(s =>s.Ddts_In)
                .ThenInclude(s => s.Product)
                .ThenInclude(s => s.Client)
                .Include(s => s.Batch)
                .ThenInclude(s => s.BatchOperations)
                .ThenInclude(s => s.Operations)
                .SingleOrDefault(s => s.SubBatchID == Id);
            sb.Status = OrderStatusConstants.STATUS_WORKING;
            for (int i = 0; i < numbersOperator; i++)
            {
                _dbContext.OperationTimelines.Add(new OperationTimeline()
                {
                    BatchOperationID = batchOperationId,
                    StartDate = start,
                    EndDate = start,
                    MachineId = machineId,
                    Status = "A",
                    SubBatch = sb,
                    SubBatchID = sb.SubBatchID
                });
            }
            sb.Ddts_In.ForEach(s => s.Status = OrderStatusConstants.STATUS_WORKING);

            _dbContext.Update(sb);
            _dbContext.SaveChanges();
            return sb.OperationTimelines.ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw e;
        }
    }

    public OperationTimeline UpdateSubBatchStatusAndOperationTimelineEnd(int operationTimelineId, DateTime end, string status)
    {
        var op =_dbContext.OperationTimelines
            .Include(s =>s.SubBatch)
            .ThenInclude(s => s.Ddts_In)
            .SingleOrDefault(s => s.OperationTimelineID == operationTimelineId);
        if (op.Status == OperationTimelineConstant.STATUS_COMPLETED)
            throw new Exception("Lavorazione già conclusa.");
        if (op != null)
        {
            var controlTime = end.DayOfYear - op.StartDate.DayOfYear;
            if ( controlTime> 0)
            {
                op.Status = "C";
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
                            Status = i == controlTime ? status : "C",
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
                            Status = i == controlTime ? status : "C",
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
            .Include(s => s.Batch)
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
            .Where(s => s.Status == OperationTimelineConstant.STATUS_WORKING || s.Status == OperationTimelineConstant.STATUS_PAUSE)
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