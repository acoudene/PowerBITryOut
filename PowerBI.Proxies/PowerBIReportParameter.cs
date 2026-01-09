// Changelogs Date  | Author                | Description
// 2023-12-23       | Anthony Coudène       | Creation

namespace PowerBI.Proxies;

public record PowerBIReportParameter
{
  public required string ParameterName { get; init; }

  public required string ReportName { get; init; }
}