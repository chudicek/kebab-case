namespace Kebabos.Models;
public class StoreWithRating
{
    public Store Store { get; }
    public long RatingSum { get; }
    public long StatusAccuracySum { get; }
    public long RatingCount { get; }

    public StoreWithRating(Store store, long ratingSum, long statusAccuracySum, long ratingCount)
    {
        Store = store;
        RatingSum = ratingSum;
        StatusAccuracySum = statusAccuracySum;
        RatingCount = ratingCount;
    }
}