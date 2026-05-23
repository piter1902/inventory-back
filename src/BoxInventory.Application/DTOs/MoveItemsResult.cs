namespace BoxInventory.Application.DTOs;

public record MoveItemsResult(
    int TotalItems,
    int SuccessCount,
    int FailureCount,
    List<ItemMoveResult> Results);

public record ItemMoveResult(
    string ItemId,
    string ItemName,
    bool Success,
    string? Error);
