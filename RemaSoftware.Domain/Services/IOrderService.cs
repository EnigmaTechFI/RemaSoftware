using RemaSoftware.Domain.Models;

namespace RemaSoftware.Domain.Services
{
    public interface IOrderService
    {
        Order GetOrderWithOperationsById(int orderId);
        void AddOrderOperation(int orderId, List<int> operationId);
        int GetTotalProcessedPiecese();
        int GetCountOrdersNotExtinguished();
        decimal GetLastMonthEarnings();
        List<Ddt_In> GetOrdersNearToDeadlineTakeTop(int topSelector);
        List<Ddt_In> GetAllOrdersNearToDeadline();
        Order GetOrderBySKU(string sku);
        void UpdateOrderStatus(int orderId, int outgoing_orders);
        IEnumerable<Order> GetOrdersNotCompleted();
        List<Order> GetOrdersCompleted();
        List<Operation> GetOperationsByOrderId(int orderId);
        Batch GetBatchById(int batchId);
        Batch GetBatchByProductIdAndOperationList(int productId, List<int> operationId);
        Batch CreateBatch(Batch batch); 
        Ddt_In CreateDDtIn(Ddt_In ddt_In);
        Ddt_In UpdateDDtIn(Ddt_In ddt_In);
        List<Ddt_In> GetAllDdtIn();
        List<Ddt_In> GetDdtInActive();
        List<Ddt_In> GetDdtInWorkingByClientId(int Id);
        List<Ddt_In> GetDdtInEnded();
        List<Ddt_In> GetDdtInStockByClientId(int Id);
        Ddt_In GetDdtInById(int id);
        List<Ddt_Out> GetDdtOutsByClientIdAndStatus(int id, string status);
        Ddt_Out CreateNewDdtOut(Ddt_Out ddtOut);
        List<Ddt_Out> GetDdtOutsByStatus(string status);
        Ddt_Out GetDdtOutById(int id);
        void UpdateDdtOut(Ddt_Out ddt);
        Ddt_Out GetDdtOutsById(int id);
        Ddt_Out CreateDDTOut(Ddt_Out ddtOut);
        void UpdateDdtAssociationByIdWithNewDdtOut(int ddtAssociationId, int ddtOutDdtOutId);
        void DeleteDDT(Ddt_In ddt);
        void DeleteSubBatch(SubBatch subBatch);
        void DeleteBatch(Batch batch);
    }
}