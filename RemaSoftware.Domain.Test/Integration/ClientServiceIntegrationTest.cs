using System;
using Microsoft.AspNetCore.Identity;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;
using RemaSoftware.Domain.Services.Impl;
using Xunit;

namespace RemaSoftware.Domain.Test.Integration;

public class ClientServiceIntegrationTest : IClassFixture<IntegrationTestFixture>, IDisposable
{
    private readonly IntegrationTestFixture _fixture;
    private readonly IClientService _sut;

    public ClientServiceIntegrationTest(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _sut = new ClientService(fixture.DbContext, null);
    }

    [Fact]
    public void GivenClientsOnDb_ShouldRetrieveOneById()
    {
        var client = _sut.GetClient(1);
        Assert.NotNull(client);
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }
}