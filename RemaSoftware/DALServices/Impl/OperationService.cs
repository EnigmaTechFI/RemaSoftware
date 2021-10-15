using System;
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

        public bool EditOrderOperations(int orderId, List<int> operationToAdd, List<int> operationToRemove)
        {
            foreach (var addOperId in operationToAdd)
                _dbContext.Order_Operations.Add(new Order_Operation {OrderID = orderId, OperationID = addOperId});
            
            _dbContext.SaveChanges();
            var opersToRemove = _dbContext.Order_Operations.Where(w => operationToRemove.Contains(w.OperationID) && w.OrderID == orderId);
            _dbContext.RemoveRange(opersToRemove);

            _dbContext.SaveChanges();
            return true;
        }
    }
}