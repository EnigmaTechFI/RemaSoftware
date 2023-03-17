using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;
using RemaSoftware.WebApp.DTOs;
using RemaSoftware.WebApp.Hub;

namespace RemaSoftware.WebApp.Helper;

public class SubBatchHelper
{
    private readonly ISubBatchService _subBatchService;
    private readonly ProductionHub _productionHub;

    public SubBatchHelper(ISubBatchService subBatchService, ProductionHub productionHub)
    {
        _subBatchService = subBatchService;
        _productionHub = productionHub;
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
        var result = _subBatchService.GetSubBatchByIdForMobile(id);
        return result;
    }

    public async Task<List<int>> StartOperationOnSubBatch(int id, int machineId, int batchOperationId, int numberOperators)
    {
        var result = await _subBatchService.UpdateSubBatchStatusAndOperationTimelineStart(id, machineId, batchOperationId, numberOperators, DateTime.Now);
        foreach (var item in result)
        {
            var paDtos = new ProductionAnalysisDto()
            {
                SubBatchId = item.SubBatchID,
                Seconds = (int)(DateTime.Now - item.StartDate).TotalSeconds,
                OperationTimeLineId = item.OperationTimelineID,
                OperationName = item.BatchOperation.Operations.Name,
                ClientName = item.SubBatch.Ddts_In[0].Product.Client.Name,
                MachineId = item.MachineId
            };
            _productionHub.StartOperation(paDtos);
        }
        return result.Select(s => s.OperationTimelineID).ToList();
    }

    public string EndOperationOnSubBatch(int operationTimelineId)
    {
        var result = _subBatchService.UpdateSubBatchStatusAndOperationTimelineEnd(operationTimelineId, DateTime.Now);
        _productionHub.EndOperation(operationTimelineId, result.MachineId);
        return "Success";
    }

}