using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RemaSoftware.Data;
using RemaSoftware.Models.HomeViewModel;

namespace RemaSoftware.Helper
{
    public class DashboardHelper
    {
        private readonly ApplicationDbContext _dbContext;
        private List<string> Colors = new List<string>
        {
            "#4e73df", "#1cc88a", "#36b9cc", "#858796", "#f6c23e"
        };

        public DashboardHelper(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
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
            var ordersPrices = _dbContext.Orders
                .Where(w=>w.DataOut.Year == DateTime.Now.Year)
                .OrderBy(ob=>ob.DataOut.Month)
                .Select(s=>new {Month=s.DataOut.ToString("MMMM"), Totals = s.Price_Uni*s.Number_Piece})
                .ToList();
            
            var couples = ordersPrices
                .GroupBy(gb=>gb.Month)
                .Select(s=>
                new ChartDataObject
                {
                    Label = s.Key,
                    Value = s.Sum(sum=>sum.Totals).ToString("0.00")
                }).ToList();
            
            var allMonths = DateTimeFormatInfo.CurrentInfo.MonthNames.Take(12).ToList();
            foreach (var month in allMonths)
            {
                if(!couples.Select(s=>s.Label).Contains(month))
                    couples.Add(new ChartDataObject { Label = month, Value = "0.00"});
            }
            return couples.OrderBy(ob=>DateTime.ParseExact(ob.Label, "MMMM", new CultureInfo("it-IT"))).ToList();

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
        
    }
}