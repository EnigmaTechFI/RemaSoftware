using RemaSoftware.Domain.Data;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.Domain.Services.Impl;

public class SupplierService : ISupplierService
{
    private readonly ApplicationDbContext _dbContext;

    public SupplierService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<Supplier> GetSuppliers()
    {
        return _dbContext.Suppliers.ToList();
    }

    public void Add(Supplier entity)
    {
        _dbContext.Suppliers.Add(entity);
        _dbContext.SaveChanges();
    }

    public void Update(Supplier entity)
    {
        _dbContext.Suppliers.Update(entity);
        _dbContext.SaveChanges();
    }

    public Supplier GetSupplierById(int supplierId)
    {
        return _dbContext.Suppliers.SingleOrDefault(s => s.SupplierID == supplierId);
    }
}