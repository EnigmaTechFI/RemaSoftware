using System;
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

    public void StartOperationOnSubBatch(int id, int machineId, int operationId, int numberOperators)
    {
        _subBatchService.UpdateSubBatchStatusAndOperationTimelineStart(id, machineId, operationId, numberOperators, DateTime.Now);
        
    }
}