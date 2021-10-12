using RemaSoftware.ContextModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemaSoftware.Models.ClientViewModel
{
    public class NewOrderViewModel
    {
        public List<Client> Clients { get; set; }
        public Order Order { get; set; }

        public List<Order> OldOrders { get; set; }
        public List<OperationFlag> Operation { get; set; }
        public string Photo { get; set; }

    }

    public class OperationFlag
    {
        public Operation Operation { get; set; }
        public bool Flag { get; set; }
    }
}
