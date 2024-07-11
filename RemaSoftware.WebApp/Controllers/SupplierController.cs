using System;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using RemaSoftware.Domain.Constants;
using RemaSoftware.WebApp.Helper;

namespace RemaSoftware.WebApp.Controllers;

[Authorize(Roles = Roles.Admin +"," + Roles.Dipendente +"," + Roles.DipendenteControl)]
public class SupplierController : Controller
{
    private readonly SupplierHelper _supplierHelper;
    private readonly INotyfService _notyfToastService;
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public SupplierController(SupplierHelper supplierHelper)
    {
        _supplierHelper = supplierHelper;
    }

    [HttpGet]
    public IActionResult List()
    {
        try
        {
            return View(_supplierHelper.GetListViewModel());
        }
        catch (Exception e)
        {
            Logger.Error(e, e.Message);
            return RedirectToAction("ClientList", "Client");
        }
    }
    
    [HttpGet]
    public IActionResult Sync()
    {
        try
        {
            _supplierHelper.SyncSupplier();
            return RedirectToAction("List");
        }
        catch (Exception e)
        {
            Logger.Error(e, e.Message);
            return RedirectToAction("List");
        }
    }
    
    [HttpGet]
    public IActionResult Info(int supplierId)
    {
        try
        {
            return View(_supplierHelper.GetInfoViewModel(supplierId));
        }
        catch (Exception e)
        {
            Logger.Error(e, e.Message);
            return RedirectToAction("List");
        }
    }
    
    
}