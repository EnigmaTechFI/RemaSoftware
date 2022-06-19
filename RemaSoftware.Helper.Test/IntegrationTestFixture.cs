using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Data;

namespace RemaSoftware.Helper.Test;

public class IntegrationTestFixture : IDisposable
{
    public ApplicationDbContext DbContext { get; set; }
    public IConfiguration Configuration { get; }
    
    public IntegrationTestFixture()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json").Build();
            
        Configuration = new ConfigurationManager();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(config["ConnectionStrings:TestConnection"])
            .Options;

        this.DbContext = new ApplicationDbContext(options);
        DbContext.Database.EnsureCreated();

        var clinte1 = this.DbContext.Clients.Add(new Client
        {
            Name = "Cliente1",
            Street = "via",
            Cap = "50100",
            City = "Firenze",
            Nation = "Italy",
            Province = "FI",
            P_Iva = "123123123123123",
            StreetNumber = "70"
        });
        this.DbContext.Orders.Add(new Order()
        {
            Name = "Order1",
            Status = "A",
            DataIn = DateTime.Now,
            DataOut = DateTime.Now.AddDays(10),
            DDT ="DDT1",
            Number_Piece = 100,
            Number_Pieces_InStock = 100,
            Price_Uni = 15,
            SKU = "SKU1",
            Client = clinte1.Entity,
            Image_URL = ""
        });
        
        DbContext.SaveChanges();
    }
    
    public void Dispose()
    {
        DbContext.Database.EnsureDeleted();
    }
}