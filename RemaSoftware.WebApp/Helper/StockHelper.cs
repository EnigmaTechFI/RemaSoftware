using System;
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

    public StockHelper(IWarehouseStockService warehouseStockService)
    {
        _warehouseStockService = warehouseStockService;
    }

    public List<Warehouse_Stock> GetAllStocks()
    {
        try
        {
            return _warehouseStockService.GetAllWarehouseStocks();
                
        }
        catch (Exception ex)
        {
            throw new Exception();
        }

    }
    
    public StockListViewModel GetStockListViewModel()
    {
        return new StockListViewModel
        {
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
        _warehouseStockService.UpdateStockArticle(stockArticle);

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
        try
        {
            _warehouseStockService.UpdateStockArticle(model.WarehouseStock);
            return "Success";
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    
    
}