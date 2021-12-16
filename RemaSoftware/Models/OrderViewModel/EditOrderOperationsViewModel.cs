using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using RemaSoftware.Models.Common;

namespace RemaSoftware.Models.OrderViewModel
{
    public class EditOrderOperationsViewModel
    {
        public List<SelectListItem> Operations { get; set; }
        public List<string> OperationsSelected { get; set; }
        public int OrderId { get; set; }

        public EditOrderOperationsViewModel()
        {
            this.Operations = new List<SelectListItem>();
        }
    }
}