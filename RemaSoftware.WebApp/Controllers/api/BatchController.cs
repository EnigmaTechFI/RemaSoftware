using Microsoft.AspNetCore.Mvc;

namespace RemaSoftware.WebApp.Controllers.api;

[Route("api/v1/[controller]/[action]")]
public class BatchController : ControllerBase
{
    //https://localhost:44328/api/v1/batch/1
    [HttpGet("{id}")]
    public IActionResult ExitFromStock(int id)
    {
        return Ok();
    }
}