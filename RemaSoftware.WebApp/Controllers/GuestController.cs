using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RemaSoftware.Domain.Models;
using RemaSoftware.WebApp.Helper;

namespace RemaSoftware.WebApp.Controllers;

public class GuestController : Controller
{
    private readonly GuestHelper _guestHelper;
    private readonly UserManager<MyUser> _userManager;

    public GuestController(GuestHelper guestHelper, UserManager<MyUser> userManager)
    {
        _guestHelper = guestHelper;
        _userManager = userManager;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        return View(_guestHelper.GetIndexViewModel((await _userManager.GetUserAsync(this.User)).Id));
    }
}