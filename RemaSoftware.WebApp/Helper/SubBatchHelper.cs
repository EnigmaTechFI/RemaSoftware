using System;
using RemaSoftware.Domain.Constants;
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
}