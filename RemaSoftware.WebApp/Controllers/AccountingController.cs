using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.DALServices;
using RemaSoftware.WebApp.Helper;
using RemaSoftware.WebApp.Models.AccountingViewModel;

namespace RemaSoftware.WebApp.Controllers
{
    public class AccountingController : Controller
    {

        private readonly AccountingHelper _accountingHelper;
        private readonly IOrderService _orderService;



        public AccountingController(AccountingHelper accountingHelper, IOrderService orderService)
        {
            _accountingHelper = accountingHelper;
            _orderService = orderService;
        }


        [Authorize(Roles = Roles.Admin)]
        public IActionResult Accounting()
        {
            var vm = new AccountingViewModel();
            vm.ordersNotCompleted = _orderService.GetOrdersNotCompleted().OrderBy(s => s.Client.Name).ToList();
            vm.orderInFactoryGroupByClient = _accountingHelper.GetDataForAccounting();

            return View(vm);
        }

        public IActionResult DownloadPdfAccounting()
        {
            var vm = new AccountingViewModel();

            vm.ordersNotCompleted = _orderService.GetOrdersNotCompleted().OrderBy(s => s.Client.Name).ToList();
            vm.orderInFactoryGroupByClient = _accountingHelper.GetDataForAccounting();

            return View("../Pdf/Accounting", vm);
        }
    }
}