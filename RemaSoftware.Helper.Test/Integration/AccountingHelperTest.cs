using System;
using RemaSoftware.Domain.Services.Impl;
using RemaSoftware.WebApp.Helper;
using Xunit;

namespace RemaSoftware.Helper.Test.Integration;

public class AccountingHelperTest : IClassFixture<IntegrationTestFixture>, IDisposable
{
    private readonly IntegrationTestFixture _fixture;
    private readonly AccountingHelper _sut;
    
    public AccountingHelperTest(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _sut = new AccountingHelper(new OrderService(_fixture.DbContext));
    }

    [Fact]
    public void Call_GetAccountingViewModel()
    {
        var result = _sut.GetAccountingViewModel();
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }
}