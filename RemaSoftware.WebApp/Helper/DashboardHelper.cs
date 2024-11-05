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

            var deliveredOrders = _dbContext.Ddts_In
                .Where(w => w.DataIn.Year == DateTime.Now.Year && w.Status == OrderStatusConstants.STATUS_DELIVERED)
                .Select(s => new
                {
                    Month = s.DataIn.ToString("MMMM", cultureInfo),
                    s.IsReso,
                    s.FreeRepair,
                    Number_Piece = s.Number_Piece,
                    NumberLostPiece = s.NumberLostPiece,
                    NumberMissingPiece = s.NumberMissingPiece,
                    NumberWastePiece = s.NumberWastePiece,
                    NumberZama = s.NumberZama,
                    NumberReturnDiscard = s.NumberReturnDiscard
                })
                .ToList();

            var allData = deliveredOrders
                .GroupBy(gb => gb.Month)
                .Select(s => new ChartDataPiecesObject
                {
                    Label = s.Key,
                    Value = new PieceData
                    {
                        Number_Piece = s.Where(x => !x.IsReso && !x.FreeRepair).Sum(sum => sum.Number_Piece),
                        NumberResoPiece = s.Where(x => x.IsReso).Sum(sum => sum.Number_Piece),
                        NumberFreeRepairPiece = s.Where(x => x.FreeRepair).Sum(sum => sum.Number_Piece),
                        NumberLostPiece = s.Sum(sum => sum.NumberLostPiece),
                        NumberMissingPiece = s.Sum(sum => sum.NumberMissingPiece),
                        NumberWastePiece = s.Sum(sum => sum.NumberWastePiece),
                        NumberZama = s.Sum(sum => sum.NumberZama),
                        NumberReturnDiscard = s.Sum(sum => sum.NumberReturnDiscard)
                    },
                })
                .ToList();

            var allMonths = cultureInfo.DateTimeFormat.MonthNames.Take(12).ToList();
            foreach (var month in allMonths)
            {
                if (!allData.Any(x => x.Label == month))
                {
                    allData.Add(new ChartDataPiecesObject
                    {
                        Label = month,
                        Value = new PieceData
                        {
                            Number_Piece = 0,
                            NumberLostPiece = 0,
                            NumberMissingPiece = 0,
                            NumberWastePiece = 0,
                            NumberZama = 0,
                            NumberReturnDiscard = 0,
                            NumberResoPiece = 0,
                            NumberFreeRepairPiece = 0
                        },
                    });
                }
            }

            return allData
                .OrderBy(ob => DateTime.ParseExact(ob.Label, "MMMM", cultureInfo))
                .ToList();
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