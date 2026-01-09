using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;
using PowerBI.Proxies.AppOwnsData.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class AadService
{
  private readonly AzureAd azureAd;
  private readonly IPublicClientApplication? publicClientApp;
  private readonly IConfidentialClientApplication? confidentialClientApp;

  public AadService(IOptions<AzureAd> azureAdOptions)
  {
    azureAd = azureAdOptions.Value;

    if (azureAd.AuthenticationMode.Equals("masteruser", StringComparison.InvariantCultureIgnoreCase))
    {
      publicClientApp = PublicClientApplicationBuilder.Create(azureAd.ClientId)
        .WithAuthority(azureAd.AuthorityUrl)
        .WithRedirectUri("http://localhost") // nécessaire pour MSAL
        .Build();

      // Ajout du cache persistant
      var storageProps = new StorageCreationPropertiesBuilder("msal_cache.dat", AppDomain.CurrentDomain.BaseDirectory).Build();
      var cacheHelper = MsalCacheHelper.CreateAsync(storageProps).GetAwaiter().GetResult();
      cacheHelper.RegisterCache(publicClientApp.UserTokenCache);
    }
    else if (azureAd.AuthenticationMode.Equals("serviceprincipal", StringComparison.InvariantCultureIgnoreCase))
    {
      var tenantSpecificUrl = azureAd.AuthorityUrl.Replace("organizations", azureAd.TenantId);

      confidentialClientApp = ConfidentialClientApplicationBuilder.Create(azureAd.ClientId)
        .WithClientSecret(azureAd.ClientSecret)
        .WithAuthority(tenantSpecificUrl)
        .Build();
    }
  }

  public async Task<AuthenticationResult?> GetAccessToken()
  {
    AuthenticationResult? result = null;

    if (azureAd.AuthenticationMode.Equals("masteruser", StringComparison.InvariantCultureIgnoreCase))
    {
      var accounts = await publicClientApp!.GetAccountsAsync();

      try
      {
        result = await publicClientApp.AcquireTokenSilent(azureAd.ScopeBase, accounts.FirstOrDefault())
                                      .ExecuteAsync();
      }
      catch (MsalUiRequiredException)
      {
#pragma warning disable CS0618 
        result = await publicClientApp.AcquireTokenByUsernamePassword(
                    azureAd.ScopeBase,
                    azureAd.PbiUsername,
                    azureAd.PbiPassword
                 ).ExecuteAsync();
#pragma warning restore CS0618

        // It is better to use this service principal in production but we can also use this (not obsolete)
        //result = await publicClientApp.AcquireTokenWithDeviceCode(azureAd.ScopeBase, deviceCodeCallback =>
        //{
        //  Console.WriteLine(deviceCodeCallback.Message);
        //  return Task.CompletedTask;
        //}).ExecuteAsync();

      }
    }
    else if (azureAd.AuthenticationMode.Equals("serviceprincipal", StringComparison.InvariantCultureIgnoreCase))
    {
      result = await confidentialClientApp!.AcquireTokenForClient(azureAd.ScopeBase).ExecuteAsync();
    }

    return result;
  }
}
