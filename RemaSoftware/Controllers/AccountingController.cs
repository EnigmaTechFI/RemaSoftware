using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RemaSoftware.Constants;

namespace RemaSoftware.Controllers
{
    public class AccountingController : Controller
    {
        [Authorize(Roles = Roles.Admin)]
        public IActionResult Accounting()
        {
            return View();
        }
    }
}