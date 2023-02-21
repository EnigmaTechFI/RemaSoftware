using RemaSoftware.Domain.Models;

namespace RemaSoftware.Domain.Services;

public interface ISubBatchService
{
    public void UpdateSubBatch(SubBatch entity);
    public void CreateSubBatch(SubBatch entity);
    public List<SubBatch> GetSubBatchesStatus(string status);
}