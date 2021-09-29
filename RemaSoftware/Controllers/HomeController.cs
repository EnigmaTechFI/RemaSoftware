using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RemaSoftware.ContextModels;
using RemaSoftware.Data;
using RemaSoftware.Models;
using RemaSoftware.Models.HomeViewModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace RemaSoftware.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<MyUser> _userManager;
        private readonly ApplicationDbContext _applicationDbContext;

        public HomeController(UserManager<MyUser> userManager, ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager;
            this._applicationDbContext = applicationDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //Recupero User
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //var user2 = _applicationDbContext.MyUsers.SingleOrDefault(i => i.Id == userId);

            //Recupero User2
            var a = User.Identity.Name;

            var user = await _userManager.FindByNameAsync(a);

            HomeViewModel model = new HomeViewModel();

            model.Username = user.UserName;
            model.UserId = user.Id;
            
            return View(model);
        }

        [HttpPost]                                  //controllata
        public IActionResult Index(HomeViewModel model)
        {
            var user = _applicationDbContext.MyUsers.SingleOrDefault(i => i.Id == model.UserId);

            model.Username = user.UserName;
            model.UserId = model.UserId;
            
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
