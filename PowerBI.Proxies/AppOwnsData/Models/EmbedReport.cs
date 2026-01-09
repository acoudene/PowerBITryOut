// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// ----------------------------------------------------------------------------

namespace PowerBI.Proxies.AppOwnsData.Models;

using System;

public record EmbedReport
{
  // Id of Power BI report to be embedded
  public required Guid ReportId { get; set; }

  // Name of the report
  public required string ReportName { get; set; }

  // Embed URL for the Power BI report
  public required string EmbedUrl { get; set; }
}
