using System.Linq;
using RemaSoftware.Domain.ContextModels;
using RemaSoftware.Domain.DALServices;
using RemaSoftware.WebApp.Controllers;
using RemaSoftware.WebApp.DTOs;
using RemaSoftware.WebApp.Models.StockViewModel;

namespace RemaSoftware.WebApp.Helper;

public class StockHelper
{
    private readonly IWarehouseStockService _warehouseService;

    public StockHelper(IWarehouseStockService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    public StockListViewModel GetStockListViewModel()
    {
        var stocks = _warehouseService.GetAllWarehouseStocks();
        
        return new StockListViewModel
        {
            WarehouseStocks = stocks.Select(s=>new StockViewModel
            {
                StockArticleId = s.Warehouse_StockID,
                Name = s.Name,
                Brand = s.Brand,
                Size = s.Size,
                Price_Uni = s.Price_Uni,
                Number_Piece = s.Number_Piece
            }).ToList()
        };
    }

    public void AddStockProduct(StockViewModel model)
    {
        model.Size = string.IsNullOrEmpty(model.Size)
            ? "Unica"
            : model.Size;
        _warehouseService.AddOrUpdateWarehouseStock(new Warehouse_Stock
        {
            Name = model.Name,
            Brand = model.Brand,
            Size = model.Size,
            Number_Piece = model.Number_Piece,
            Price_Uni = model.Price_Uni
        });
    }

    public void EditStockArticle(Warehouse_Stock model)
    {
        _warehouseService.AddOrUpdateWarehouseStock(model);
    }

    public void DeleteStockArticle(int stockArticleId)
    {
        _warehouseService.DeleteWarehouseStockById(stockArticleId);
    }

    public StockJsonResultDTO AddOrRemoveQuantityFromArticle(QtyAddRemoveJsDTO model)
    {
        if (model.ArticleId <= default(int))
            return new StockJsonResultDTO(false,"Articolo mancante.");
        if (model.QtyToAddRemove <= 0)
            return new StockJsonResultDTO(false,"La quantità deve essere maggiore di 0.");
        
        var stockArticle = _warehouseService.GetStockArticleById(model.ArticleId);
                
        if(model.QtyToAddRemoveRadio == 0 && stockArticle.Number_Piece - model.QtyToAddRemove < 0)
            return new StockJsonResultDTO(false, "La quantità risulterebbe minore di 0.");
                
        stockArticle.Number_Piece = model.QtyToAddRemoveRadio == 1
            ? stockArticle.Number_Piece + model.QtyToAddRemove
            : stockArticle.Number_Piece - model.QtyToAddRemove;
        _warehouseService.UpdateStockArticle(stockArticle);

        return new StockJsonResultDTO
        {
            Result = true,
            ToastMessage = "Quantità modificata correttamente.",
            Number_Piece = stockArticle.Number_Piece,
            Price_Tot = stockArticle.Price_Tot
        };
    }
}