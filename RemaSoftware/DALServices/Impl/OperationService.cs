using System.Collections.Generic;
using System.Linq;
using RemaSoftware.ContextModels;
using RemaSoftware.Data;

namespace RemaSoftware.DALServices.Impl
{
    public class OperationService : IOperationService
    {
        private readonly ApplicationDbContext _dbContext;

        public OperationService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public List<Operation> GetAllOperations()
        {
            return _dbContext.Operations.ToList();
        }

        public void AddOperation(Operation operation)
        {
            _dbContext.Add(operation);
            _dbContext.SaveChanges();
        }
    }
}