﻿using RemaSoftware.Domain.Models;

namespace RemaSoftware.Domain.Services;

public interface ISubBatchService
{
    public void UpdateSubBatch(SubBatch entity);
    public void UpdateSubBatchStatus(int Id, string status);
    public Task<List<OperationTimeline>> UpdateSubBatchStatusAndOperationTimelineStart(int Id, int machineId, int batchOperationId, int numbersOperator, DateTime start);
    public OperationTimeline UpdateSubBatchStatusAndOperationTimelineEnd(int operationTimelineId, DateTime end, string status);
    public void CreateSubBatch(SubBatch entity);
    public List<SubBatch> GetSubBatchesStatus(string status);
    public List<SubBatch> GetSubBatchesStatusForOrderSummary(string status);
    public SubBatch GetSubBatchById(int id);
    public List<OperationTimeline> GetOperationTimelinesByStatus(string status);
    public List<OperationTimeline> GetSubBatchAtQualityControl();
    public SubBatch GetSubBatchByIdForMobile(int id);
    public List<OperationTimeline> GetOperationTimelinesByMulitpleStatus(List<string> status);
    public List<OperationTimeline> GetOperationTimelinesForMachine(int machineId);
    public List<SubBatch> GetSubBatchesStatusAndClientId(string status, int clientId);
    public SubBatch GetSubBatchByIdAndClientId(int id, int clientId);
    public List<SubBatch> GetSubBatchesWithExtras();
    public List<Ddt_Supplier> GetSubBatchToSupplier();
    public List<SubBatch> GetSubBatchByBatchId(int SubBatchId);
}