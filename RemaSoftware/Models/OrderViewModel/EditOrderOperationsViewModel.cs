using System.Collections.Generic;
using RemaSoftware.Models.Common;

namespace RemaSoftware.Models.OrderViewModel
{
    public class EditOrderOperationsViewModel
    {
        public List<OperationFlag> OrderOperations { get; set; }
        public int OrderId { get; set; }

        public EditOrderOperationsViewModel()
        {
            this.OrderOperations = new List<OperationFlag>();
        }
    }
}