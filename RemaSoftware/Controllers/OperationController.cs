using Microsoft.AspNetCore.Mvc;
using RemaSoftware.Models.OperationViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemaSoftware.Controllers
{
    public class OperationController : Controller
    {

        public OperationController()
        {

        }

        [HttpGet]
        public IActionResult AddOperation()
        {
            var vm = new AddOperationViewModel();
            return View(vm);
        }

        [HttpPost]
        public IActionResult AddOperation(AddOperationViewModel model)
        {
            return View(model);
        }
    }
}
