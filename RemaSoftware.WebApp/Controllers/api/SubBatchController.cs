using System;
using Microsoft.AspNetCore.Mvc;
using RemaSoftware.WebApp.Helper;

namespace RemaSoftware.WebApp.Controllers.api;

[Route("api/v1/[controller]/[action]")]
public class SubBatchController : ControllerBase
{
    private readonly SubBatchHelper _batchHelper;

    public SubBatchController(SubBatchHelper batchHelper)
    {
        _batchHelper = batchHelper;
    }

    //https://localhost:44328/api/v1/SubBatch/ExitFromStock/1
    [HttpGet("{id}")]
    public IActionResult ExitFromStock(int id)
    {
        try
        {
            _batchHelper.ExitFormStock(id);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}