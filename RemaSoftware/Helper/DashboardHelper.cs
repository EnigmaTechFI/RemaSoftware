using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RemaSoftware.ContextModels;
using RemaSoftware.DALServices;
using RemaSoftware.Data;
using RemaSoftware.Models.HomeViewModel;

namespace RemaSoftware.Helper
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

        public List<ChartDataObject> GetDataForDashboardPieChart()
        {
            var customers = _dbContext.Clients
                .Include(i=>i.Orders)
                .Where(w=>w.Orders.Count > 0)
                .OrderByDescending(ob => ob.Orders.Count())
                .Take(5).Select(s => 
                    new ChartDataObject
                    {
                        Label = s.Name,
                        Value = s.Orders.Count.ToString()
                    })
                .ToList();
            this.SetBackgroundColorsForPieChart(ref customers);
            return customers;
        }
        
        public List<ChartDataObject> GetDataForDashboardAreaChart()
        {
            CultureInfo cultureInfo = new CultureInfo("it-IT");
            var ordersPrices = _dbContext.Orders
                .Where(w=>w.DataOut.Year == DateTime.Now.Year)
                .OrderBy(ob=>ob.DataOut.Month)
                .Select(s=>new {Month=s.DataOut.ToString("MMMM", cultureInfo), Totals = s.Price_Uni*s.Number_Piece})
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
                .GroupBy(gb=> new{gb.Name, gb.Brand, gb.Size})
                .Select(s=>s.First()).ToList();
            
            return articlesInStock;
        }

        public List<ChartDataObject> GetDataForBarChartDashboard()
        {
            var result = 
                _orderService.GetAllOrdersNearToDeadline()
                .GroupBy(gb=>gb.ClientID).ToList()
                .Select(s=> new ChartDataObject
                {
                    Label = s.First().Client.Name,
                    Value = s.Sum(sum=>sum.Number_Piece).ToString()
                })
                .ToList();
            return result;
        }
    }
}