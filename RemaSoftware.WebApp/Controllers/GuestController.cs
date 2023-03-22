using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NLog;
using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.Models;
using RemaSoftware.WebApp.Helper;
using RemaSoftware.WebApp.Models.OrderViewModel;

namespace RemaSoftware.WebApp.Controllers;
[Authorize(Roles = Roles.Cliente)]
public class GuestController : Controller
{
    private readonly GuestHelper _guestHelper;
    private readonly UserManager<MyUser> _userManager;
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public GuestController(GuestHelper guestHelper, UserManager<MyUser> userManager)
    {
        _guestHelper = guestHelper;
        _userManager = userManager;
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
    public IActionResult DdtEmitted()
    {
        throw new System.NotImplementedException();
    }
}