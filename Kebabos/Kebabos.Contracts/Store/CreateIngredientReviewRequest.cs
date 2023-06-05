namespace Kebabos.Contracts.Store;

public record CreateIngredientReviewRequest(int Rating, int StatusAccuracy, string? Comment, Guid UserId);