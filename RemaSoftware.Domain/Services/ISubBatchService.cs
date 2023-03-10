using RemaSoftware.Domain.Models;

namespace RemaSoftware.Domain.Services;

public interface ISubBatchService
{
    public void UpdateSubBatch(SubBatch entity);
    public void UpdateSubBatchStatus(int Id, string status);
    public Task<List<int>> UpdateSubBatchStatusAndOperationTimelineStart(int Id, int machineId, int batchOperationId, int numbersOperator, DateTime start);
    public string UpdateSubBatchStatusAndOperationTimelineEnd(int operationTimelineId, DateTime end);
    public void CreateSubBatch(SubBatch entity);
    public List<SubBatch> GetSubBatchesStatus(string status);
    public SubBatch GetSubBatchById(int id);
    public List<OperationTimeline> GetOperationTimelinesByStatus(string status);
    public List<OperationTimeline> GetSubBatchAtQualityControl();
}