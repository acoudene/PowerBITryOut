// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// ----------------------------------------------------------------------------

namespace PowerBI.Proxies.AppOwnsData.Models;

using Microsoft.PowerBI.Api.Models;
using System.Collections.Generic;

public record EmbedParams
{
  // Type of the object to be embedded
  public required string Type { get; set; }

  // Report to be embedded
  public required List<EmbedReport> EmbedReport { get; set; }

  // Embed Token for the Power BI report
  public required EmbedToken EmbedToken { get; set; }
}
