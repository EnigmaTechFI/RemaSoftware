using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;
using RemaSoftware.UtilityServices.Implementation;
using RemaSoftware.WebApp.DTOs;
using RemaSoftware.WebApp.Models.StockViewModel;


namespace RemaSoftware.WebApp.Helper;

public class StockHelper
{
    private readonly IWarehouseStockService _warehouseStockService;
    private readonly ISupplierService _supplierService;
    private readonly EmailService _emailService;
    private readonly UserManager<MyUser> _userManager;

    public StockHelper(IWarehouseStockService warehouseStockService, ISupplierService supplierService, UserManager<MyUser> userManager, EmailService emailService)
    {
        _warehouseStockService = warehouseStockService;
        _supplierService = supplierService;
        _emailService = emailService;
        _userManager = userManager;
    }

    public List<Warehouse_Stock> GetAllStocks()
    {
        return _warehouseStockService.GetAllWarehouseStocks();
    }

    public StockListViewModel GetStockListViewModel(bool newProduct)
    {
        return new StockListViewModel
        {
            newProduct = newProduct,
            WarehouseStocks = GetAllStocks()
        };
    }
    
    public async Task<Warehouse_Stock> AddStockProduct(NewStockViewModel model)
    {
        return _warehouseStockService.AddStockProduct(model.WarehouseStock);
    }

    public void DeleteStockArticle(int stockArticleId)
    {
        _warehouseStockService.DeleteWarehouseStockById(stockArticleId);
    }

    public StockJsonResultDTO AddOrRemoveQuantityFromArticle(QtyAddRemoveJsDTO model)
    {
        if (model.ArticleId <= default(int))
            return new StockJsonResultDTO(false,"Articolo mancante.");
        if (model.QtyToAddRemove <= 0)
            return new StockJsonResultDTO(false,"La quantità deve essere maggiore di 0.");
        
        var stockArticle = _warehouseStockService.GetStockArticleById(model.ArticleId);
                
        if(model.QtyToAddRemoveRadio == 0 && stockArticle.Number_Piece - model.QtyToAddRemove < 0)
            return new StockJsonResultDTO(false, "La quantità risulterebbe minore di 0.");
                
        stockArticle.Number_Piece = model.QtyToAddRemoveRadio == 1
            ? stockArticle.Number_Piece + model.QtyToAddRemove
            : stockArticle.Number_Piece - model.QtyToAddRemove;
        _warehouseStockService.UpdateStockQuantity(stockArticle, model.QtyToAddRemove, model.QtyToAddRemoveRadio);


        var admins = _userManager.GetUsersInRoleAsync("Admin").Result;
        var adminEmails = admins.Select(u => u.Email).ToList();

        foreach (var mail in adminEmails)
        {
            if (stockArticle.Number_Piece < stockArticle.Reorder_Limit)
            {
                _emailService.SendEmailStock(stockArticle.Warehouse_StockID, stockArticle.Name,
                    stockArticle.Supplier.Name, mail);
            }
        }

        return new StockJsonResultDTO
        {
            Result = true,
            ToastMessage = "Quantità modificata correttamente.",
            Number_Piece = stockArticle.Number_Piece,
            Price_Tot = stockArticle.Price_Tot
        };
    }
    
    public NewStockViewModel GetStockById(int id)
    {
        return new NewStockViewModel()
        {
            WarehouseStock = _warehouseStockService.GetStockArticleById(id),
            UnitMeasure = WarehouseStockMeasure.GetUnitMeasure()
        };
    }

    public async Task<string> EditStock(StockViewModel model)
    {
        _warehouseStockService.UpdateStockArticle(model.WarehouseStock);
        return "Success";
    }


    public NewStockViewModel GetAddProductViewModel()
    {
        var vm = new NewStockViewModel
        {
            Suppliers = _supplierService.GetSuppliers(),
            UnitMeasure = WarehouseStockMeasure.GetUnitMeasure()
        };
        return vm;
    }

    public ReportStockViewModel ReportStock()
    {
        var vm = new ReportStockViewModel
        {
            StockHistories =  _warehouseStockService.GetReportStocks()
        };
        return vm;
    }
}