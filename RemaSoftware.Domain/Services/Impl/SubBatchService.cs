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

    public async Task<List<int>> UpdateSubBatchStatusAndOperationTimelineStart(int Id, int machineId, int batchOperationId, int numbersOperator, DateTime start)
    {
        try
        {
            var sb = _dbContext.SubBatches
                .Include(s =>s.Ddts_In)
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
            return sb.OperationTimelines.Select(s => s.OperationTimelineID).ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw e;
        }
    }

    public void UpdateSubBatchStatusAndOperationTimelineEnd(int Id, int machineId, int batchOperationId, int numbersOperator,
        DateTime end)
    {
        var sb =_dbContext.SubBatches
            .Include(s =>s.Ddts_In)
            .Include(s => s.OperationTimelines.Where(s => s.BatchOperationID == batchOperationId && s.MachineId == machineId && s.Status == "A"))
            .SingleOrDefault(s => s.SubBatchID == Id);
        for (int i = 0; i < numbersOperator; i++)
        {
            sb.OperationTimelines[i].Status = "C";
            sb.OperationTimelines[i].EndDate = end;
        }

        _dbContext.SubBatches.Update(sb);
        _dbContext.SaveChanges();
    }

    public void UpdateSubBatchStatusAndOperationTimelinePause(int Id, int machineId, int batchOperationId, int numbersOperator,
        DateTime end)
    {
        var sb =_dbContext.SubBatches
            .Include(s =>s.Ddts_In)
            .Include(s => s.OperationTimelines.Where(s => s.BatchOperationID == batchOperationId && s.MachineId == machineId && s.Status == "A"))
            .SingleOrDefault(s => s.SubBatchID == Id);
        for (int i = 0; i < numbersOperator; i++)
        {
            sb.OperationTimelines[i].Status = "B";
            sb.OperationTimelines[i].EndDate = end;
        }

        _dbContext.SubBatches.Update(sb);
        _dbContext.SaveChanges();
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
}