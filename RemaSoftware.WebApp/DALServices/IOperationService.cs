using System.Collections.Generic;
using RemaSoftware.WebApp.ContextModels;

namespace RemaSoftware.WebApp.DALServices
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