using Microsoft.EntityFrameworkCore;
using NLog;
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
}