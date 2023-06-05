namespace Kebabos.Contracts.Store;

public record CreateStoreReviewRequest(int Rating, int StatusAccuracy, string? Comment, Guid UserId);