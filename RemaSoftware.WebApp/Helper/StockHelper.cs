using System.Collections.Generic;
using System.Threading.Tasks;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;
using RemaSoftware.WebApp.DTOs;
using RemaSoftware.WebApp.Models.StockViewModel;


namespace RemaSoftware.WebApp.Helper;

public class StockHelper
{
    private readonly IWarehouseStockService _warehouseStockService;
    private readonly ISupplierService _supplierService;

    public StockHelper(IWarehouseStockService warehouseStockService, ISupplierService supplierService)
    {
        _warehouseStockService = warehouseStockService;
        _supplierService = supplierService;
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
            WarehouseStock = _warehouseStockService.GetStockArticleById(id)
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
            Suppliers = _supplierService.GetSuppliers()
        };
        return vm;
    }

}