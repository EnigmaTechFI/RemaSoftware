﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NLog;
using RemaSoftware.WebApp.Helper;

namespace RemaSoftware.WebApp.Controllers.api;

[Route("api/v1/[controller]/[action]")]
public class SubBatchController : ControllerBase
{
    private readonly SubBatchHelper _batchHelper;
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public SubBatchController(SubBatchHelper batchHelper)
    {
        _batchHelper = batchHelper;
    }

    //https://localhost:44328/api/v1/SubBatch/Detail/1
    [HttpGet("{id}")]
    public JsonResult Detail(int id)
    {
        try
        {
            return new JsonResult(new {Data = _batchHelper.GetSubBatchDetail(id), Error = ""});
        }
        catch (Exception e)
        {
            Logger.Error(e, $"Detail Operation API: {e.Message}");
            return new JsonResult(new {Data = "ERORR", Error = e.Message});
        }
    }
    
    
    //https://localhost:44328/api/v1/SubBatch/Start?id=1&machineId=1&batchOperationId=1&numberOperators
    [HttpGet]
    public async Task<JsonResult> Start(int id, int machineId, int batchOperationId, int numberOperators)
    {
        try
        {
            return new JsonResult(new {Data = await _batchHelper.StartOperationOnSubBatch(id, machineId, batchOperationId, numberOperators), Error = ""});
        }
        catch (Exception e)
        {
            Logger.Error(e, $"Start Operation API: {e.Message}");
            return new JsonResult(new {Data = "ERROR", Error = e.Message});
        }
    }
    
    //https://localhost:44328/api/v1/SubBatch/End?operationTimelineId=1
    [HttpGet]
    public JsonResult End(int operationTimelineId)
    {
        try
        {
            return new JsonResult(new {Data= "SUCCESS", Time = _batchHelper.EndOperationOnSubBatch(operationTimelineId), Error = ""});
        }
        catch (Exception e)
        {
            Logger.Error(e, $"End Operation API: {e.Message}");
            return new JsonResult(new {Data = "ERROR", Error = e.Message});
        }
    }
    
    //https://localhost:44328/api/v1/SubBatch/Pause?operationTimelineId=1
    [HttpGet]
    public JsonResult Pause(int operationTimelineId)
    {
        try
        {
            return new JsonResult(new {Data ="SUCCESS", Time = _batchHelper.PauseOperationOnSubBatch(operationTimelineId), Error = ""});
        }
        catch (Exception e)
        {
            Logger.Error(e,$"Pause Operation API: {e.Message}");
            return new JsonResult(new {Data = "ERROR", Error = e.Message});
        }
    }
    
    //https://localhost:44328/api/v1/SubBatch/GetOperationsTimeline?machineId=1
    [HttpGet]
    public JsonResult GetOperationsTimeline(int machineId)
    {
        try
        {
            return new JsonResult(new {Data = _batchHelper.GetOperationsTimelineByMachineId(machineId), Error = ""});
        }
        catch (Exception e)
        {
            Logger.Error(e,$"Get Operations Timeline API: {e.Message}");
            return new JsonResult(new {Data = "ERROR", Error = e.Message});
        }
    }
}