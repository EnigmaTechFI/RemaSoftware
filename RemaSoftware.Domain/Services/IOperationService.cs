using RemaSoftware.Domain.Models;

namespace RemaSoftware.Domain.Services
{
    public interface IOperationService
    {
        List<Operation> GetAllOperations();
        List<Operation> GetAllOperationsWithOutCOQAndEXTRA();
        void AddOperation(Operation operation);
        List<Operation> AddOperations(List<Operation> operations);
        Operation GetOperationById(int id);
        void UpdateOperation(Operation Operation);
        int GetOperationIdByName(string name);
    }
}