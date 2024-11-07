using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;
using RemaSoftware.Domain.Data;
using RemaSoftware.WebApp.Models.HomeViewModel;

namespace RemaSoftware.WebApp.Helper
{
    public class DashboardHelper
    {
        
        private readonly ApplicationDbContext _dbContext;
        private readonly IOrderService _orderService;

        private List<string> Colors = new List<string>
        {
            "#4e73df", "#1cc88a", "#36b9cc", "#858796", "#f6c23e"
        };

        public DashboardHelper(ApplicationDbContext dbContext, IOrderService orderService)
        {
            _dbContext = dbContext;
            _orderService = orderService;
        }
        
        public List<ChartDataObject> GetDataForDashboardAreaChart()
        {
            CultureInfo cultureInfo = new CultureInfo("it-IT");
            var ordersPrices = _dbContext.Ddts_In
                .Include(s => s.SubBatch)
                .ThenInclude(s => s.Batch)
                .Where(w=>w.DataIn.Year == DateTime.Now.Year && w.IsReso == false)
                .OrderBy(ob=>ob.DataIn.Month)
                .Select(s=>new {Month=s.DataIn.ToString("MMMM", cultureInfo), Totals = s.Price_Uni*s.Number_Piece})
                .ToList();
            
            var couples = ordersPrices
                .GroupBy(gb=>gb.Month)
                .Select(s=>
                new ChartDataObject
                {
                    Label = s.Key,
                    Value = s.Sum(sum=>sum.Totals).ToString("0.00")
                }).ToList();
            
            var allMonths = cultureInfo.DateTimeFormat.MonthNames.Take(12).ToList();
            foreach (var month in allMonths)
            {
                if(!couples.Select(s=>s.Label).Contains(month))
                    couples.Add(new ChartDataObject { Label = month, Value = "0.00"});
            }
            return couples.OrderBy(ob=>DateTime.ParseExact(ob.Label, "MMMM", cultureInfo)).ToList();

        }
        
       public List<ChartDataPiecesObject> GetDataForDashboardAreaChartPieces()
        {
            CultureInfo cultureInfo = new CultureInfo("it-IT");

            // Preleva i pezzi da Ddts_In (solo quelli FreeRepair e Reso)
            var deliveredOrders = _dbContext.Ddts_In
                .Where(w => w.DataIn.Year == DateTime.Now.Year && w.Status == OrderStatusConstants.STATUS_DELIVERED)
                .GroupBy(s => s.DataIn.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    ResoPieces = g.Where(x => x.IsReso).Sum(x => x.Number_Piece),
                    FreeRepairPieces = g.Where(x => x.FreeRepair).Sum(x => x.Number_Piece),
                    LostPieces = g.Where(x => x.NumberLostPiece > 0).Sum(x => x.NumberLostPiece),
                    MissingPieces = g.Where(x => x.NumberMissingPiece > 0).Sum(x => x.NumberMissingPiece),
                    WastePieces = g.Where(x => x.NumberWastePiece > 0).Sum(x => x.NumberWastePiece),
                    ZamaPieces = g.Where(x => x.NumberZama > 0).Sum(x => x.NumberZama),
                    ReturnDiscardPieces = g.Where(x => x.NumberReturnDiscard > 0).Sum(x => x.NumberReturnDiscard)
                })
                .ToList();

            // Preleva i dati da Ddt_Associations (già aggregato per tipo di pezzo)
            var associationOrders = _dbContext.Ddt_Associations
                .Where(da => da.Ddt_In.DataIn.Year == DateTime.Now.Year)
                .GroupBy(da => new { da.Ddt_In.DataIn.Month, da.TypePieces })
                .Select(g => new
                {
                    g.Key.Month,
                    g.Key.TypePieces,
                    TotalPieces = g.Sum(x => x.NumberPieces)
                })
                .ToList();

            // Prepara i dati combinati
            var allData = new List<ChartDataPiecesObject>();

            for (int month = 1; month <= 12; month++)
            {
                // Ottieni i dati aggregati da Ddts_In
                var deliveredData = deliveredOrders.FirstOrDefault(x => x.Month == month);

                // Ottieni i dati aggregati da Ddt_Associations per ciascun tipo
                var buonPieces = associationOrders.FirstOrDefault(x => x.Month == month && x.TypePieces == PiecesType.BUONI)?.TotalPieces ?? 0;
                var persiPieces = associationOrders.FirstOrDefault(x => x.Month == month && x.TypePieces == PiecesType.PERSI)?.TotalPieces ?? 0;
                var mancantiPieces = associationOrders.FirstOrDefault(x => x.Month == month && x.TypePieces == PiecesType.MANCANTI)?.TotalPieces ?? 0;
                var scartiPieces = associationOrders.FirstOrDefault(x => x.Month == month && x.TypePieces == PiecesType.SCARTI)?.TotalPieces ?? 0;
                var zamaPieces = associationOrders.FirstOrDefault(x => x.Month == month && x.TypePieces == PiecesType.ZAMA)?.TotalPieces ?? 0;
                var resoScartoPieces = associationOrders.FirstOrDefault(x => x.Month == month && x.TypePieces == PiecesType.RESOSCARTO)?.TotalPieces ?? 0;

                // Aggiungi i dati nel risultato
                allData.Add(new ChartDataPiecesObject
                {
                    Label = cultureInfo.DateTimeFormat.GetMonthName(month),
                    Value = new PieceData
                    {
                        Number_Piece = buonPieces,
                        NumberResoPiece = deliveredData?.ResoPieces ?? 0,
                        NumberFreeRepairPiece = deliveredData?.FreeRepairPieces ?? 0,
                        NumberLostPiece = persiPieces,
                        NumberMissingPiece = mancantiPieces,
                        NumberWastePiece = scartiPieces,
                        NumberZama = zamaPieces,
                        NumberReturnDiscard = resoScartoPieces
                    }
                });
            }

            return allData;
        }

        
        private void SetBackgroundColorsForPieChart(ref List<ChartDataObject> chartDataObjects)
        {
            for (int i = 0; i < chartDataObjects.Count; i++)
            {
                var ranCol = Colors[new Random().Next(Colors.Count)];
                chartDataObjects[i].BackgroundColor = ranCol;
                Colors.Remove(ranCol);
            }
        }

        public List<Warehouse_Stock> GetAllWarehouseStocksForDashboard()
        {
            var articlesInStock = _dbContext.Warehouse_Stocks
                .Where(w => w.Number_Piece > 0)
                .ToList()
                .GroupBy(gb=> new{gb.Name})
                .Select(s=>s.First()).ToList();
            
            return articlesInStock;
        }

        public List<ChartDataObject> GetDataForBarChartDashboard()
        {
            var result = 
                _orderService.GetAllOrdersNearToDeadline()
                .GroupBy(gb=>gb.Product.ClientID).ToList()
                .Select(s=> new ChartDataObject
                {
                    Label = s.First().Product.Client.Name,
                    Value = s.Sum(sum=>sum.Number_Piece_Now).ToString()
                })
                .ToList();
            return result;
        }
    }
}