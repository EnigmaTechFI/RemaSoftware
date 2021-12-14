using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RemaSoftware.Constants;
using RemaSoftware.Helper;
using RemaSoftware.Models.AccountingViewModel;

namespace RemaSoftware.Controllers
{
    public class AccountingController : Controller
    {

        private readonly AccountingHelper _accountingHelper;


        public AccountingController(AccountingHelper accountingHelper)
        {
            _accountingHelper = accountingHelper;
        }


        [Authorize(Roles = Roles.Admin)]
        public IActionResult Accounting()
        {
            var vm = new AccountingViewModel();
            vm.orderInFactoryGroupByClient = _accountingHelper.GetDataForAccounting();

            return View(vm);
        }
    }
}