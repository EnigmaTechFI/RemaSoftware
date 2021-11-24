using RemaSoftware.Constants;
using RemaSoftware.ContextModels;

namespace RemaSoftware.Models.OrderViewModel
{
    public class ChangeOrderStatusModel
    {
        public Order Order { get; set; }
        public StatusDto CurrentStatus { get; set; }
        public StatusDto NewStatus { get; set; }
    }
}