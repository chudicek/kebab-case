namespace Kebabos.Models;

public class StoreRating
{
    public Guid Id { get; }
    public Guid StoreId { get; }
    public Guid UserId { get; }
    public int Rating { get; }
    public int StatusAccuracy { get; }
    public string? Comment { get; }

    public StoreRating(Guid id, Guid storeId, Guid userId, int rating, int statusAccuracy, string? comment)
    {
        Id = id;
        StoreId = storeId;
        UserId = userId;
        Rating = rating;
        StatusAccuracy = statusAccuracy;
        Comment = comment;
    }
}