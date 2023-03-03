using RemaSoftware.Domain.Models;

namespace RemaSoftware.Domain.Services;

public interface ISubBatchService
{
    public void UpdateSubBatch(SubBatch entity);
    public void UpdateSubBatchStatus(int Id, string status);
    public void UpdateSubBatchStatusAndOperationTimelineStart(int Id, int machineId, int batchOperationId, int numbersOperator, DateTime start);
    public void CreateSubBatch(SubBatch entity);
    public List<SubBatch> GetSubBatchesStatus(string status);
    public SubBatch GetSubBatchById(int id);
}