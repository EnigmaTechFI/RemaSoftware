using System;
using RemaSoftware.Domain.ContextModels;
using RemaSoftware.Domain.DALServices;
using RemaSoftware.Domain.DALServices.Impl;
using Xunit;

namespace RemaSoftware.Domain.Test.Integration;

public class ClientServiceIntegrationTest : IClassFixture<IntegrationTestFixture>, IDisposable
{
    private readonly IntegrationTestFixture _fixture;
    private readonly IClientService _sut;

    public ClientServiceIntegrationTest(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _sut = new ClientService(fixture.DbContext);
    }

    [Fact]
    public void GivenClientsOnDb_ShouldRetrieveOneById()
    {
        var client = _sut.GetClient(1);
        Assert.NotNull(client);
    }

    public void Dispose()
    {
    }
}