using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.ContextModels;

namespace RemaSoftware.WebApp.Models.OrderViewModel
{
    public class ChangeOrderStatusModel
    {
        public Order Order { get; set; }
        public StatusDto CurrentStatus { get; set; }
        public StatusDto NewStatus { get; set; }
    }
}