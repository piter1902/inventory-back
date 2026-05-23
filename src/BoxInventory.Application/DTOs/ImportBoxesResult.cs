namespace BoxInventory.Application.DTOs;

public record ImportBoxesResult(
    int TotalSheets,
    int SuccessCount,
    int FailureCount,
    List<BoxImportResult> Results);

public record BoxImportResult(
    string SheetName,
    string? BoxName,
    bool Success,
    string? Error);
