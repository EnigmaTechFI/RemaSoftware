using RemaSoftware.Domain.Models;

namespace RemaSoftware.Domain.Services;

public interface ISupplierService
{
    List<Supplier> GetSuppliers();
    void Add(Supplier entity);
    void Update(Supplier entity);
    Supplier GetSupplierById(int supplierId);
}