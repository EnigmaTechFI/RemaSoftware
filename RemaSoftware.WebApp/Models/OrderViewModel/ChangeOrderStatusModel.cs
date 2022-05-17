using RemaSoftware.WebApp.Constants;
using RemaSoftware.WebApp.ContextModels;

namespace RemaSoftware.WebApp.Models.OrderViewModel
{
    public class ChangeOrderStatusModel
    {
        public Order Order { get; set; }
        public StatusDto CurrentStatus { get; set; }
        public StatusDto NewStatus { get; set; }
    }
}