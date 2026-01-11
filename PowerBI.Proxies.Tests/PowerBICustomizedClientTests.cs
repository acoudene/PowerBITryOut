// Changelogs Date  | Author                | Description
// 2023-12-23       | Anthony Coudène       | Creation

using Microsoft.Extensions.Options;
using Microsoft.PowerBI.Api;
using PowerBI.Proxies.AppOwnsData.Models;
using PowerBI.Proxies.AppOwnsData.Services;

namespace PowerBI.Proxies.Tests;

public class PowerBICustomizedClientTests
{
  private static PbiEmbedService GetPbiEmbedService()
  {
    var azureAd = new AzureAd
    {
      AuthenticationMode = "serviceprincipal", // masteruser or serviceprincipal
      AuthorityUrl = "https://login.microsoftonline.com/organizations",
      ClientId = "<todo_guid>",
      ScopeBase = new[] { "https://analysis.windows.net/powerbi/api/.default" },
      ClientSecret = "<todo_client_secret>"
    };

    var options = Options.Create(azureAd);
    var aadService = new AadService(options);
    var service = new PbiEmbedService(aadService);
    return service;
  }  

  [Fact]
  public async Task GetPowerBIClient_can_give_an_instance_from_connection_settings()
  {
    // Arrange
    PbiEmbedService service = GetPbiEmbedService();

    // Act
    using var client = await service.GetPowerBIClient();

    // Assert
    Assert.NotNull(client);
    Assert.IsType<PowerBIClient>(client);
  }

}