using System.Collections.Generic;
using RemaSoftware.ContextModels;

namespace RemaSoftware.DALServices
{
    public interface IOperationService
    {
        List<Operation> GetAllOperations();
        bool EditOrderOperations(int orderId, List<int> operationToAdd, List<int> operationToRemove);

        void AddOperation(Operation operation);
        List<Operation> AddOperations(List<Operation> operations);
        void RemoveAllOrderOperations(int orderId);
    }
}