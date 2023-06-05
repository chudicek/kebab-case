namespace Kebabos.Models;

public class IngredientWithRating
{
    public Ingredient Ingredient { get; }
    public long RatingSum { get; }
    public long StatusAccuracySum { get; }
    public long RatingCount { get; }

    public IngredientWithRating(Ingredient ingredient, long ratingSum, long statusAccuracySum, long ratingCount)
    {
        Ingredient = ingredient;
        RatingSum = ratingSum;
        StatusAccuracySum = statusAccuracySum;
        RatingCount = ratingCount;
    }
}