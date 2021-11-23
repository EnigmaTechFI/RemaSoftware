using System.Collections.Generic;
using RemaSoftware.ContextModels;

namespace RemaSoftware.DALServices
{
    public interface IOrderService
    {
        List<Order> GetAllOrders();
        Order GetOrderById(int orderId);
        Order GetOrderWithOperationsById(int orderId);
        Order AddOrder(Order order);
        List<Order> GetOrdersByFilters();
        void AddOrderOperation(int orderId, List<int> operationId);
        int GetTotalProcessedPiecese();
        int GetCountOrdersNotExtinguished();
        decimal GetLastMonthEarnings();
        List<Order> GetOrdersNearToDeadlineTakeTop(int topSelector);
        List<Order> GetAllOrdersNearToDeadline();
        List<string> GetOldOrders_SKU();
        Order GetOrderBySKU(string sku);
        bool UpdateOrder(Order order);
        void DeleteOrderByID(int OrderID);
        void UpdateOrderStatus(int orderId, string newStatus);
        List<Order> GetOrdersNotCompleted();
    }
}