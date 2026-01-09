// Changelogs Date  | Author                | Description
// 2023-12-23       | Anthony Coudène       | Creation

using Microsoft.PowerBI.Api;
using System.Threading;
using System.Threading.Tasks;

namespace PowerBI.Proxies.AppOwnsData.Services;

public partial class PbiEmbedService
{
  public async Task<string?> FindParameterValueInDatasetInGroupAsync(
    string groupName,
    string datasetName,
    string parameterName,
    CancellationToken cancellationToken = default)
  {
    using PowerBIClient pbiClient = await GetPowerBIClient();
    var parameter = await pbiClient.FindParameterInDatasetInGroupAsync(groupName, datasetName, parameterName, cancellationToken: cancellationToken);
    if (parameter == null)
    {
      return null;
    }

    return parameter.CurrentValue;
  }
}
