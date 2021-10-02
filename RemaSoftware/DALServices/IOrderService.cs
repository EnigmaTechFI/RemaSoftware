using System.Collections.Generic;
using RemaSoftware.ContextModels;

namespace RemaSoftware.DALServices
{
    public interface IOrderService
    {
        List<Order> GetAllOrders();
        Order GetOrderById(int orderId);
        void AddOrder(Order order);
        List<Order> GetOrdersByFilters();
    }
}