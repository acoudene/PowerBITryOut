appsettings.json

```json
{
	{
  "AzureAd": {
    "AuthenticationMode": "MasterUser",
    "AuthorityUrl": "https://login.microsoftonline.com/organizations/",
    "ClientId": "",
    "TenantId": "",
    "ScopeBase": ["https://analysis.windows.net/powerbi/api/.default"],
    "PbiUsername": "",
    "PbiPassword": "",
    "ClientSecret": ""
  },
  "PowerBI": {
    "WorkspaceId": "",
    "ReportId": ""
  }
}
```

Program.cs

```csharp
// Register AadService and PbiEmbedService for dependency injection
services
.AddScoped(typeof(AadService))
.AddScoped(typeof(PbiEmbedService));

// Loading appsettings.json in C# Model classes
services
.Configure<AzureAd>(Configuration.GetSection("AzureAd"))
.Configure<PowerBI>(Configuration.GetSection("PowerBI"));
```