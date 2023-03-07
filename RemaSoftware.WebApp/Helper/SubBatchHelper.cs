using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;

namespace RemaSoftware.WebApp.Helper;

public class SubBatchHelper
{
    private readonly ISubBatchService _subBatchService;

    public SubBatchHelper(ISubBatchService subBatchService)
    {
        _subBatchService = subBatchService;
    }

    public void ExitFormStock(int id)
    {
        try
        {
            _subBatchService.UpdateSubBatchStatus(id, OrderStatusConstants.STATUS_WORKING);
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public SubBatch GetSubBatchDetail(int id)
    {
        return _subBatchService.GetSubBatchById(id);
    }

    public async Task<List<int>> StartOperationOnSubBatch(int id, int machineId, int batchOperationId, int numberOperators)
    {
        return await _subBatchService.UpdateSubBatchStatusAndOperationTimelineStart(id, machineId, batchOperationId, numberOperators, DateTime.Now);
    }

    public string EndOperationOnSubBatch(int operationTimelineId)
    {
        return _subBatchService.UpdateSubBatchStatusAndOperationTimelineEnd(operationTimelineId, DateTime.Now);
    }

}