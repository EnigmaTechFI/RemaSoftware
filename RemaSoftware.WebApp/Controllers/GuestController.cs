using System;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NLog;
using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.Models;
using RemaSoftware.WebApp.Helper;

namespace RemaSoftware.WebApp.Controllers;
[Authorize(Roles = Roles.Cliente)]
public class GuestController : Controller
{
    private readonly GuestHelper _guestHelper;
    private readonly UserManager<MyUser> _userManager;
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly INotyfService _notyfToastService;

    public GuestController(GuestHelper guestHelper, UserManager<MyUser> userManager, INotyfService notyfToastService)
    {
        _guestHelper = guestHelper;
        _userManager = userManager;
        _notyfToastService = notyfToastService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View(_guestHelper.GetIndexViewModel((await _userManager.GetUserAsync(this.User)).Id));
    }
    
    [HttpGet]
    public async Task<IActionResult> OrdersActive()
    {
        try
        {
            return View(_guestHelper.GetOrdersActiveViewModel((await _userManager.GetUserAsync(this.User)).Id));
        }
        catch (Exception e)
        {
            Logger.Error(e, e.Message);
            return RedirectToAction("Index");
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> OrdersEnded()
    {
        try
        {
            return View(_guestHelper.GetOrdersEndedViewModel((await _userManager.GetUserAsync(this.User)).Id));
        }
        catch (Exception e)
        {
            Logger.Error(e, e.Message);
            return RedirectToAction("Index");
        }
    }

    [HttpGet]
    public async Task<IActionResult> BatchInStock()
    {
        try
        {
            return View(_guestHelper.GetBatchInStockViewModel((await _userManager.GetUserAsync(this.User)).Id));
        }
        catch (Exception e)
        {
            Logger.Error(e, e.Message);
            return RedirectToAction("Index");
        }
    }

    [HttpGet]
    public async Task<IActionResult> BatchInProduction()
    {
        try
        {
            return View(_guestHelper.GetBatchInProductionViewModel((await _userManager.GetUserAsync(this.User)).Id));
        }
        catch (Exception e)
        {
            Logger.Error(e, e.Message);
            return RedirectToAction("Index");
        }
    }

    [HttpGet]
    public async Task<IActionResult> BatchInDelivery()
    {
        try
        {
            return View(_guestHelper.GetBatchInDeliveryViewModel((await _userManager.GetUserAsync(this.User)).Id));
        }
        catch (Exception e)
        {
            Logger.Error(e, e.Message);
            return RedirectToAction("Index");
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> SubBatchMonitoring(int id)
    {
        try
        {
            return View(_guestHelper.GetSubBatchMonitoring(id, (await _userManager.GetUserAsync(this.User)).Id));
        }
        catch (Exception e)
        {
            Logger.Error(e, e.Message);
            _notyfToastService.Error(e.Message);
            return RedirectToAction("Index");
        }
    }
    
    [HttpGet]
    public async Task<JsonResult> PromptDdt(int id, string note)
    {
        try
        {
            if (note.Length > 500)
            {
                return new JsonResult(new {Result = true, Error = "Errore, nota troppo lunga. Si prega di riprovare."});
            }
 
            if (note.Length == 0)
            { 
                note = "Nessuna nota";
            }
            
            var users = await _userManager.GetUsersInRoleAsync(Roles.Admin);
            _guestHelper.SendPrompt(id, users, (await _userManager.GetUserAsync(this.User)).Id, note);
            return new JsonResult(new {Result = true, Error = "DDT sollecitata correttamente."});
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
            return new JsonResult(new {Result = true, Error = "Errore durante l'invio del sollecito. Si prega di riprovare."});
        }
    }
}