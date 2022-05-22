using System;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RemaSoftware.Domain.ContextModels;
using RemaSoftware.Domain.Data;

namespace RemaSoftware.Domain.Test;

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

        this.DbContext.Clients.Add(new Client()
        {
            Name = "Antani",
            Street = "via",
            Cap = "50100",
            City = "Firenze",
            Nation = "Italy",
            Province = "FI",
            P_Iva = "123123123123123",
            StreetNumber = "70"
        });
        
        DbContext.SaveChanges();
    }
    
    public void Dispose()
    {
        DbContext.Database.EnsureDeleted();
    }
}