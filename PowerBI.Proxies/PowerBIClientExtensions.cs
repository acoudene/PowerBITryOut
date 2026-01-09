// Changelogs Date  | Author                | Description
// 2023-12-23       | Anthony Coudène       | Creation

using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PowerBI.Proxies;

public static class PowerBIClientExtensions
{
  /// <summary>
  /// Retrieves a collection of groups (workspaces) from the Power BI service.
  /// </summary>
  /// <remarks>This method is an extension method for the <see cref="PowerBIClient"/> class and provides a
  /// convenient way to retrieve groups (workspaces) asynchronously. The returned <see cref="Groups"/> object contains
  /// metadata about the groups, such as their IDs and names.</remarks>
  /// <param name="powerBIClient">The <see cref="PowerBIClient"/> instance used to interact with the Power BI service.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Groups"/> object
  /// representing the collection of groups (workspaces) retrieved from the Power BI service.</returns>
  public static async Task<Groups> GetGroupsAsync(
    this PowerBIClient powerBIClient, 
    CancellationToken cancellationToken = default)
  {        
    return await powerBIClient.Groups.GetGroupsAsync(cancellationToken: cancellationToken);
  }
  
  /// <summary>
  /// Asynchronously finds a Power BI group by its name.
  /// </summary>
  /// <remarks>This method performs a case-insensitive search for a group with the specified name. If multiple
  /// groups with the same name exist, an exception will be thrown due to the use of <see
  /// cref="Enumerable.SingleOrDefault"/>.</remarks>
  /// <param name="powerBIClient">The <see cref="PowerBIClient"/> instance used to query the groups.</param>
  /// <param name="groupName">The name of the group to search for. This parameter cannot be <see langword="null"/>, empty, or whitespace.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete. This parameter is optional.</param>
  /// <returns>A <see cref="Group"/> object representing the group with the specified name, or <see langword="null"/> if no
  /// matching group is found.</returns>
  /// <exception cref="ArgumentNullException">Thrown if <paramref name="groupName"/> is <see langword="null"/>, empty, or consists only of whitespace.</exception>
  public static async Task<Group?> FindGroupAsync(
    this PowerBIClient powerBIClient, 
    string groupName, 
    CancellationToken cancellationToken = default)
  {    
    if (string.IsNullOrWhiteSpace(groupName))
      throw new ArgumentNullException(nameof(groupName));

    var groups = await powerBIClient.GetGroupsAsync(cancellationToken);
    if (groups is null || groups.Value is null)
      return null;

    return groups.Value
      .SingleOrDefault(group => group.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase));
  }

  /// <summary>
  /// Finds a Power BI group by its name and retrieves the datasets associated with it.
  /// </summary>
  /// <remarks>This method first searches for a group with the specified name using the Power BI client.  If the
  /// group is found, it retrieves the datasets associated with the group.  If no group is found, the method returns a
  /// tuple with both elements set to <see langword="null"/>.</remarks>
  /// <param name="powerBIClient">The <see cref="PowerBIClient"/> instance used to interact with the Power BI service.</param>
  /// <param name="groupName">The name of the group to search for. Cannot be null, empty, or whitespace.</param>
  /// <param name="cancellationToken">A token to monitor for cancellation requests. Optional.</param>
  /// <returns>A tuple containing the found group and its associated datasets.  If the group is not found, both elements of the
  /// tuple will be <see langword="null"/>.</returns>
  /// <exception cref="ArgumentNullException">Thrown if <paramref name="groupName"/> is null, empty, or consists only of whitespace.</exception>
  public static async Task<(Group?, Datasets?)> FindDatasetsInGroupAsync(
    this PowerBIClient powerBIClient, 
    string groupName, 
    CancellationToken cancellationToken = default)
  {
    if (string.IsNullOrWhiteSpace(groupName))
      throw new ArgumentNullException(nameof(groupName));

    var group = await powerBIClient.FindGroupAsync(groupName, cancellationToken);
    if (group is null)
      return (null, null);

    Guid groupId = group.Id;

    return (group, await powerBIClient.Datasets.GetDatasetsInGroupAsync(groupId, cancellationToken: cancellationToken));
  }

  /// <summary>
  /// Finds a dataset within a specified Power BI group by its name.
  /// </summary>
  /// <remarks>This method searches for a dataset by name within a specified Power BI group. The search is
  /// case-insensitive. If the group or dataset does not exist, the method returns <c>(null, null)</c>.</remarks>
  /// <param name="powerBIClient">The <see cref="PowerBIClient"/> instance used to interact with the Power BI service.</param>
  /// <param name="groupName">The name of the Power BI group to search. Cannot be null, empty, or whitespace.</param>
  /// <param name="datasetName">The name of the dataset to find within the specified group. Cannot be null, empty, or whitespace.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the operation to complete. Optional.</param>
  /// <returns>A tuple containing the group and the dataset if found. If the group or dataset is not found, returns <c>(null,
  /// null)</c>.</returns>
  /// <exception cref="ArgumentNullException">Thrown if <paramref name="groupName"/> or <paramref name="datasetName"/> is null, empty, or consists only of
  /// whitespace.</exception>
  public static async Task<(Group?, Dataset?)> FindDatasetInGroupAsync(
    this PowerBIClient powerBIClient, 
    string groupName, 
    string datasetName, 
    CancellationToken cancellationToken = default)
  {
    if (string.IsNullOrWhiteSpace(groupName))
      throw new ArgumentNullException(nameof(groupName));
    if (string.IsNullOrWhiteSpace(datasetName))
      throw new ArgumentNullException(nameof(datasetName));

    var (group, datasets) = await powerBIClient.FindDatasetsInGroupAsync(groupName, cancellationToken);
    if (group is null || datasets is null || datasets.Value is null)
      return (null, null);
    
    return (group, datasets.Value
      .SingleOrDefault(dataset => dataset.Name.Equals(datasetName, StringComparison.OrdinalIgnoreCase)));
  } 

  /// <summary>
  /// Retrieves the mashup parameters for a specified dataset within a specified group in Power BI.
  /// </summary>
  /// <remarks>This method first locates the specified dataset within the specified group. If the group or
  /// dataset cannot be found,  the method returns <see langword="null"/>. Otherwise, it retrieves the mashup parameters
  /// associated with the dataset.</remarks>
  /// <param name="powerBIClient">The <see cref="PowerBIClient"/> instance used to interact with the Power BI service.</param>
  /// <param name="groupName">The name of the group (workspace) containing the dataset. Cannot be null, empty, or whitespace.</param>
  /// <param name="datasetName">The name of the dataset for which to retrieve parameters. Cannot be null, empty, or whitespace.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the operation to complete. Optional.</param>
  /// <returns>A <see cref="MashupParameters"/> object containing the parameters of the specified dataset,  or <see
  /// langword="null"/> if the group or dataset cannot be found.</returns>
  /// <exception cref="ArgumentNullException">Thrown if <paramref name="groupName"/> or <paramref name="datasetName"/> is null, empty, or consists only of
  /// whitespace.</exception>
  public static async Task<MashupParameters?> GetParametersInDatasetInGroupAsync(
    this PowerBIClient powerBIClient, 
    string groupName, 
    string datasetName, 
    CancellationToken cancellationToken = default)
  {
    if (string.IsNullOrWhiteSpace(groupName))
      throw new ArgumentNullException(nameof(groupName));
    if (string.IsNullOrWhiteSpace(datasetName))
      throw new ArgumentNullException(nameof(datasetName));

    var (group, dataset) = await powerBIClient.FindDatasetInGroupAsync(groupName, datasetName, cancellationToken);
    if (group is null || dataset is null)
      return null;

    Guid groupId = group.Id;
    string datasetId = dataset.Id;

    return await powerBIClient.Datasets.GetParametersInGroupAsync(groupId, datasetId);
  }
  
  /// <summary>
  /// Asynchronously retrieves a specific parameter from a dataset within a specified group in Power BI.
  /// </summary>
  /// <remarks>This method searches for a parameter by name within the specified dataset in the specified group.
  /// If the dataset or parameter does not exist, the method returns <see langword="null"/>.</remarks>
  /// <param name="powerBIClient">The <see cref="PowerBIClient"/> instance used to interact with the Power BI service.</param>
  /// <param name="groupName">The name of the group (workspace) containing the dataset. Cannot be null, empty, or whitespace.</param>
  /// <param name="datasetName">The name of the dataset from which to retrieve the parameter. Cannot be null, empty, or whitespace.</param>
  /// <param name="parameterName">The name of the parameter to retrieve. The search is case-insensitive. Cannot be null, empty, or whitespace.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete. Optional.</param>
  /// <returns>A <see cref="MashupParameter"/> representing the requested parameter if found; otherwise, <see langword="null"/>.</returns>
  /// <exception cref="ArgumentNullException">Thrown if <paramref name="groupName"/>, <paramref name="datasetName"/>, or <paramref name="parameterName"/> is
  /// null, empty, or consists only of whitespace.</exception>
  public static async Task<MashupParameter?> FindParameterInDatasetInGroupAsync(
    this PowerBIClient powerBIClient, 
    string groupName, 
    string datasetName, 
    string parameterName, 
    CancellationToken cancellationToken = default)
  {
    if (string.IsNullOrWhiteSpace(groupName))
      throw new ArgumentNullException(nameof(groupName));
    if (string.IsNullOrWhiteSpace(datasetName))
      throw new ArgumentNullException(nameof(datasetName));
    if (string.IsNullOrWhiteSpace(parameterName))
      throw new ArgumentNullException(nameof(parameterName));

    var parameters = await powerBIClient.GetParametersInDatasetInGroupAsync(groupName, datasetName, cancellationToken);
    if (parameters is null || parameters.Value is null)
      return null;

    return parameters.Value
      .SingleOrDefault(param => param.Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase));
  }

}
