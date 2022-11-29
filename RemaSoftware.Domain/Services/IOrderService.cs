using System.Collections.Generic;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.Domain.Services
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
        List<Order> GetChunkedOrders(int skip, int take);
        int GetTotalOrdersCount();
        Order GetOrderBySKU(string sku);
        bool UpdateOrder(Order order);
        bool DeleteOrderByID(int OrderID);
        void UpdateOrderStatus(int orderId, int outgoing_orders);
        IEnumerable<Order> GetOrdersNotCompleted();
        List<Order> GetOrdersCompleted();
        List<Operation> GetOperationsByOrderId(int orderId);
    }
}