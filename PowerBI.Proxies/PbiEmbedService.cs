// Changelogs Date  | Author                | Description
// 2023-12-23       | Anthony Coudène       | Creation

using Microsoft.PowerBI.Api;
using System;
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

  /// <summary>
  /// Asynchronously retrieves the value of a specified parameter from a dataset within a tenant's group.
  /// </summary>
  /// <remarks>This method delegates the retrieval of the parameter value to an internal service. The <paramref
  /// name="reportName"/> parameter  is optional and may be null or empty if the dataset does not require it.</remarks>
  /// <param name="tenantId">The unique identifier of the tenant. Cannot be null, empty, or whitespace.</param>
  /// <param name="reportName">The name of the report associated with the dataset. Can be null or empty if not required by the dataset.</param>
  /// <param name="parameterName">The name of the parameter whose value is to be retrieved. Cannot be null, empty, or whitespace.</param>
  /// <returns>A task that represents the asynchronous operation. The task result contains the value of the specified parameter
  /// as a string,  or <see langword="null"/> if the parameter is not found.</returns>
  /// <exception cref="ArgumentNullException">Thrown if <paramref name="tenantId"/> or <paramref name="parameterName"/> is null, empty, or consists only of
  /// whitespace.</exception>
  public async Task<string?> FindParameterValueAsync(string tenantId, string reportName, string parameterName, CancellationToken cancellationToken = default)
  {
    if (string.IsNullOrWhiteSpace(tenantId))
      throw new ArgumentNullException(nameof(tenantId));
    if (string.IsNullOrWhiteSpace(parameterName))
      throw new ArgumentNullException(nameof(parameterName));

    string groupName = tenantId;
    string dataSetName = reportName;

    return await FindParameterValueInDatasetInGroupAsync(
      groupName,
      dataSetName,
      parameterName,
      cancellationToken);
  }

  /// <summary>
  /// Asynchronously retrieves the value of a specified parameter for a given tenant and report.
  /// </summary>
  /// <param name="tenantId">The unique identifier of the tenant. This value cannot be null, empty, or whitespace.</param>
  /// <param name="reportParameter">An object containing the report name and parameter name for which the value is to be retrieved. This value cannot
  /// be null.</param>
  /// <param name="cancellationToken"></param>
  /// <returns>A task that represents the asynchronous operation. The task result contains the value of the specified parameter,
  /// or <see langword="null"/> if the parameter value is not found.</returns>
  /// <exception cref="ArgumentNullException">Thrown if <paramref name="tenantId"/> is null, empty, or consists only of whitespace, or if <paramref
  /// name="reportParameter"/> is null.</exception>
  public async Task<string?> FindParameterValueAsync(
    string tenantId,
    PowerBIReportParameter reportParameter
    , CancellationToken cancellationToken = default)
  {
    if (string.IsNullOrWhiteSpace(tenantId))
      throw new ArgumentNullException(nameof(tenantId));
    if (reportParameter is null)
      throw new ArgumentNullException(nameof(reportParameter));

    return await FindParameterValueAsync(
      tenantId,
      reportParameter.ReportName,
      reportParameter.ParameterName,
      cancellationToken);
  }
}
