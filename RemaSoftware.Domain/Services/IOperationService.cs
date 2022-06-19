using System.Collections.Generic;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.Domain.Services
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