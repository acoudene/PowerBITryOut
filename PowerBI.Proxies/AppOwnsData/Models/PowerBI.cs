// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// ----------------------------------------------------------------------------

namespace PowerBI.Proxies.AppOwnsData.Models;

public record PowerBI
{
  // Workspace Id for which Embed token needs to be generated
  public required string WorkspaceId { get; set; }

  // Report Id for which Embed token needs to be generated
  public required string ReportId { get; set; }
}
