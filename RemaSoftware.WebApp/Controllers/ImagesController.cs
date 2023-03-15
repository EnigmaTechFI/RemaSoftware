using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using NLog;
using RemaSoftware.Domain.Services;
using RemaSoftware.Domain.Services.Impl;

namespace RemaSoftware.WebApp.Controllers;

public class ImagesController : Controller
{
    private readonly OrderImageBlobService _orderBlobService;
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly IWebHostEnvironment _environment;

    public ImagesController(IWebHostEnvironment environment, OrderImageBlobService orderBlobService)
    {
        _environment = environment;
        _orderBlobService = orderBlobService;
    }

    [HttpGet("images/{section}/{fileName}")]
    public async Task<FileResult> Get(string section, string fileName)
    {
        try
        {
            var image = await _orderBlobService.GetBlobAsync($"{section}/{fileName}");
            return File(image.BlobContent, image.BlobContentType);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, ex.Message);
            return File(await System.IO.File.ReadAllBytesAsync($"{_environment.WebRootPath}\\img\\notfound.jpg"), "image/jpeg");
        }
    }
    
}