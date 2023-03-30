using System;
using System.Linq;
using NLog;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;
using RemaSoftware.UtilityServices.Interface;
using RemaSoftware.WebApp.Models.SupplierViewModel;

namespace RemaSoftware.WebApp.Helper;

public class SupplierHelper
{
    private readonly ISupplierService _supplierService;
    private readonly IAPIFatturaInCloudService _fatturaInCloudService;
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public SupplierHelper(ISupplierService supplierService, IAPIFatturaInCloudService fatturaInCloudService)
    {
        _supplierService = supplierService;
        _fatturaInCloudService = fatturaInCloudService;
    }

    public SupplierListViewModel GetListViewModel()
    {
        return new SupplierListViewModel()
        {
            Suppliers = _supplierService.GetSuppliers()
        };
    }

    public void SyncSupplier()
    {
        var result = _fatturaInCloudService.GetListSuppliers();
        var suppliers = _supplierService.GetSuppliers();
        foreach (var item in result)
        {
            try
            {
                var entity = suppliers.SingleOrDefault(s => s.FC_SupplierID == item.FC_SupplierID);
                if (entity != null)
                {
                    entity.FC_SupplierID = item.FC_SupplierID;
                    entity.Cap = item.Cap;
                    entity.Street = item.Street;
                    entity.City = item.City;
                    entity.Pec = item.Pec;
                    entity.Email = item.Email;
                    entity.PhoneNumber = item.PhoneNumber;
                    entity.Province = item.Province;
                    entity.Fax = item.Fax;
                    entity.Name = item.Name;
                    entity.P_Iva = item.P_Iva;
                    _supplierService.Update(entity);
                }
                else
                {
                    entity = new Supplier();
                    entity.FC_SupplierID = item.FC_SupplierID;
                    entity.Cap = item.Cap;
                    entity.Street = item.Street;
                    entity.City = item.City;
                    entity.Pec = item.Pec;
                    entity.Email = item.Email;
                    entity.PhoneNumber = item.PhoneNumber;
                    entity.Province = item.Province;
                    entity.Fax = item.Fax;
                    entity.Name = item.Name;
                    entity.P_Iva = item.P_Iva;
                    _supplierService.Add(entity);
                }
            
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                continue;
            }
        }
        
    }

    public SupplierInfoViewModel GetInfoViewModel(int supplierId)
    {
        return new SupplierInfoViewModel()
        {
            Supplier = _supplierService.GetSupplierById(supplierId)
        };
    }
}