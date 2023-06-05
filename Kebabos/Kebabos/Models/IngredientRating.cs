namespace Kebabos.Models;

public class IngredientRating
{
    public Guid Id { get; }
    public Guid IngredientId { get; }
    public Guid UserId { get; }
    public int Rating { get; }
    public int StatusAccuracy { get; }
    public string? Comment { get; }

    public IngredientRating(Guid id, Guid ingredientId, Guid userId, int rating, int statusAccuracy, string? comment)
    {
        Id = id;
        IngredientId = ingredientId;
        UserId = userId;
        Rating = rating;
        StatusAccuracy = statusAccuracy;
        Comment = comment;
    }
}