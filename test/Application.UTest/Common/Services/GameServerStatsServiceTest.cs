using System.Net;
using System.Text.Json;
using Crpg.Application.Common;
using Crpg.Application.Common.Services;
using Crpg.Domain.Entities.Items;
using Crpg.Sdk.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace Crpg.Application.UTest.Common.Services;

public class GameServerStatsServiceTest : TestBase
{
    [Test]
    public async Task Basic()
    {
        // Arrange

        var mockHttp = new MockHttpMessageHandler();

        var responseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            // Content = new StringContent(JsonSerializer.Serialize("{'d':1}"))
        };


        var handlerMock = new Mock<HttpClientHandler>();

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "GetFromJsonAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        var handler = handlerMock.Object;
        var client = new HttpClient(handler);

        Mock<IConfiguration> configurationMock = new();
        configurationMock.SetupGet(x => x[It.Is<string>(s => s == "Datadog:ApiKey")]).Returns("111");
        configurationMock.SetupGet(x => x[It.Is<string>(s => s == "Datadog:ApplicationKey")]).Returns("222");

        DatadogGameServerStatsService datadogGameServerStatsService = new(configurationMock.Object, client);

        // Act
        var res = await datadogGameServerStatsService.GetGameServerStatsAsync(CancellationToken.None);

        // Assert
        Assert.That(res, Is.Empty);
    }
}
