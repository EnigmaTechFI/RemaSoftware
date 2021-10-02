using System.Collections.Generic;
using RemaSoftware.ContextModels;

namespace RemaSoftware.DALServices
{
    public interface IOperationService
    {
        List<Operation> GetAllOperations();
    }
}