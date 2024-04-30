using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.Data;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;
using RemaSoftware.UtilityServices.Interface;
using RemaSoftware.WebApp.Models.AccountingViewModel;
using static RemaSoftware.WebApp.Models.AccountingViewModel.AccountingViewModel;

namespace RemaSoftware.WebApp.Helper
{
    public class AccountingHelper
    {
        private readonly IOrderService _orderService;
        private readonly ISubBatchService _subBatchService;
        private readonly IEmailService _emailService;
        private readonly IAPIFatturaInCloudService _apiFatturaInCloudService;
        private readonly ApplicationDbContext _dbContext;
        private readonly IClientService _clientService;
        private readonly IMachineService _machineService;

        public AccountingHelper(IOrderService orderService, ISubBatchService subBatchService, IEmailService emailService, ApplicationDbContext dbContext, IAPIFatturaInCloudService apiFatturaInCloudService, IClientService clientService, IMachineService machineService)
        {
            _orderService = orderService;
            _subBatchService = subBatchService;
            _emailService = emailService;
            _dbContext = dbContext;
            _apiFatturaInCloudService = apiFatturaInCloudService;
            _clientService = clientService;
            _machineService = machineService;
        }

        public ProductionAnalysisLiveViewModel GetProductionAnalysisLiveViewModel()
        {
            var op = _subBatchService.GetOperationTimelinesByStatus(OperationTimelineConstant.STATUS_WORKING);
            var productionLiveDtos = new List<ProductionLiveDto>();
            var now = DateTime.Now;
            bool automaticMachine;
            try
            {
                automaticMachine = _machineService.ConnectMachine().Result.MachineOn;
            }
            catch (Exception ex)
            {
                automaticMachine = false; // o un altro valore di default
            }
            foreach (var item in op.Where(s => s.MachineId.HasValue).ToList())
            {
                productionLiveDtos.Add(new ProductionLiveDto()
                {
                    Client = item.SubBatch.Ddts_In[0].Product.Client.Name,
                    OperationTimelineId = item.OperationTimelineID,
                    MachineId = item.MachineId.Value,
                    Operation = item.BatchOperation.Operations.Name,
                    SubBatchId = item.SubBatchID,
                    Time = (int)(now - item.StartDate).TotalSeconds
                });
            }
            return new ProductionAnalysisLiveViewModel()
            {
                 ProductionLiveDtos = productionLiveDtos,
                 AutomaticMachine = automaticMachine
            };
        }
        public AccountingViewModel GetAccountingViewModel()
        {
            var ordersInFactory = _orderService.GetDdtInActive()
                .GroupBy(c => c.Product.ClientID)
                .Select(s =>
                    new OrderInFactoryGroupByClient
                    {
                        Client = s.First().Product.Client.Name,
                        ClientID = s.First().Product.Client.ClientID,
                        NumberPiecesInStock = s.Sum(sum => sum.Number_Piece_Now),
                        TotPriceInStock = s.Sum(sum => sum.Price_Uni * sum.Number_Piece_Now)
                    }).ToList();
            var vm = new AccountingViewModel
            {
                OrdersInFactoryGroupByClient = ordersInFactory,
                OrdersNotCompleted = _orderService.GetDdtInActive().OrderBy(s => s.Product.Client.Name).ToList()
            };
            return vm;
        }

        public DdtVariationViewModel GetDdtVariationViewModel()
        {
            return new DdtVariationViewModel()
            {
                SubBatches = _subBatchService.GetSubBatchesWithExtras()
            };
        }

        public void ConfirmVariation(int id)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var ddt = _orderService.GetDdtInById(id);
                    ddt.Price_Uni = ddt.PendingPrice;
                    ddt.PendingPrice = 0;
                    ddt.PriceIsPending = false;
                    _orderService.UpdateDDtIn(ddt);
                    _apiFatturaInCloudService.EditDdtInCloud(ddt, ddt.Product.SKU);
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }

        public void DeleteVariation(int id)
        {
            var ddt = _orderService.GetDdtInById(id);
            ddt.PendingPrice = 0;
            ddt.PriceIsPending = false;
            _orderService.UpdateDDtIn(ddt);
        }

        public void RequestVariation(int id, string price, string mail, string message)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var ddt = _orderService.GetDdtInById(id);
                    try
                    {
                        ddt.PendingPrice = Decimal.Parse(price, new CultureInfo("it-IT"));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw new Exception("Prezzo inserito non corretto.");
                    }
                    ddt.PriceIsPending = true;
                    _orderService.UpdateDDtIn(ddt);
                    _emailService.SendEmailPriceVariation(price, mail, message, ddt.Code, ddt.Product.Client.Name);
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }

        public PreliminarViewModel GetPreliminarViewModel()
        {
            return new PreliminarViewModel()
            {
                Batches = _orderService.GetAllBatch()
            };
        }

        public BatchAnalsysisViewModel GetBatchAnalsysisViewModel(int id)
        {
            var batch = _orderService.GetBatchById(id);
            var batchChart = GetBatchChart(batch);
            var opChart = GetOperationTimeForSubbatchChart(batch);
            return new BatchAnalsysisViewModel()
            {
                Batch = batch,
                AvgTime = batchChart,
                AnalysisOperationCharts = opChart
            };
        }

        private List<AnalysisOperationChart> GetOperationTimeForSubbatchChart(Batch batch)
        {
            var chart = new List<AnalysisOperationChart>();
            foreach (var item in batch.SubBatches.OrderBy(s => s.SubBatchID))
            {
                foreach (var op in item.OperationTimelines.Where(s =>
                             s.Status == OperationTimelineConstant.STATUS_COMPLETED && s.UseForStatics))
                {
                    var element = chart.SingleOrDefault(s =>
                        s.Id == item.SubBatchID && s.OperationName == op.BatchOperation.Operations.Name);
                    if (element != null)
                    {
                        element.TotTime += (int)(op.EndDate - op.StartDate).TotalSeconds;
                    }
                    else
                    {
                        var opChart = new AnalysisOperationChart()
                        {
                            Id = item.SubBatchID,
                            OperationName = op.BatchOperation.Operations.Name,
                            Piece = item.Ddts_In.Sum(s => s.Number_Piece),
                            TotTime = (int)(op.EndDate - op.StartDate).TotalSeconds,
                            Date = item.OperationTimelines.Where(s => s.BatchOperationID == op.BatchOperationID).OrderBy(s => s.StartDate).FirstOrDefault().StartDate,
                            DateSubBatch = item.OperationTimelines.Where(s => s.SubBatchID == item.SubBatchID && s.Status == OperationTimelineConstant.STATUS_COMPLETED && s.UseForStatics).OrderBy(s => s.StartDate).FirstOrDefault().StartDate.ToString("dd/MM/yy"),
                        };
                        chart.Add(opChart);
                    }

                }
            }

            return chart.OrderBy(s => s.Date).ToList();
        }
        private List<OperationChartSubBatch> GetBatchChart(Batch batch)
        {
            var chart = new List<OperationChartSubBatch>();
            foreach (var item in batch.SubBatches.Where(s => s.OperationTimelines.Any(a => a.Status == OperationTimelineConstant.STATUS_COMPLETED && a.UseForStatics)).OrderByDescending(s => s.BatchID))
            {
                var obj = new OperationChartSubBatch()
                {
                    Id = item.SubBatchID,
                    Time = item.OperationTimelines.Where(s =>
                            s.Status == OperationTimelineConstant.STATUS_COMPLETED && s.UseForStatics)
                        .Sum(s => (int)(s.EndDate - s.StartDate).TotalSeconds),
                    Date = item.OperationTimelines.Where(s =>
                        s.Status == OperationTimelineConstant.STATUS_COMPLETED && s.UseForStatics).OrderBy(s => s.StartDate).FirstOrDefault().StartDate.ToString("dd/MM/yy"),
                    Pieces = item.Ddts_In.Sum(s => s.Number_Piece)
                };
                chart.Add(obj);
            }

            return chart;
        }

        public ClientAccountingViewModel GetClientAnalsysisViewModel(int id)
        {
            return new ClientAccountingViewModel()
            {
                Ddts_In = _clientService.ClientAccountingInfo(id)
            };
        }

        public int getPieces(int selectedMonth, int selectedYear, int id)
        {
            var Ddts_In = _clientService.ClientAccountingInfo(id);

            var sum = 0;
            foreach (var output in Ddts_In)
            {
                if (output.DataIn.Month == selectedMonth && selectedMonth != 0 && output.DataIn.Year == selectedYear)
                {
                    sum += output.Number_Piece;
                }
            }
            return sum;
        }
        
        public decimal getPrice(int selectedMonth, int selectedYear, int id)
        {
            var Ddts_In = _clientService.ClientAccountingInfo(id);

            decimal sum = 0;
            foreach (var output in Ddts_In)
            {
                if (output.DataIn.Month == selectedMonth && selectedMonth != 0 && output.DataIn.Year == selectedYear)
                {
                    sum += output.Number_Piece * output.Price_Uni;
                }
            }
            return sum;
        }
    }
}
