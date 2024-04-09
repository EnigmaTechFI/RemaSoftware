using Microsoft.EntityFrameworkCore;
using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Data;

namespace RemaSoftware.Domain.Services.Impl
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

        public List<Operation> GetAllOperationsWithOutCOQAndEXTRA()
        {
            return _dbContext.Operations.Where(s => s.Name != OtherConstants.COQ && s.Name != OtherConstants.EXTRA).ToList();
        }

        public void AddOperation(Operation operation)
        {
            _dbContext.Add(operation);
            _dbContext.SaveChanges();
        }

        public List<Operation> AddOperations(List<Operation> operations)
        {
            _dbContext.Operations.AddRange(operations);
            _dbContext.SaveChanges();
            return operations;
        }

        public Operation GetOperationById(int id)
        {
            return _dbContext.Operations.SingleOrDefault(s => s.OperationID == id);
        }

        public void UpdateOperation(Operation Operation)
        {
            _dbContext.Operations.Update(Operation);
            _dbContext.SaveChanges();
        }

        public int GetOperationIdByName(string name)
        {
            return _dbContext.Operations.SingleOrDefault(s => s.Name == name).OperationID;
        }
    }
}