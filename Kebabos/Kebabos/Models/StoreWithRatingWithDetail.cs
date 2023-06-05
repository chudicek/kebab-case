namespace Kebabos.Models;
public class StoreWithRatingWithDetail
{
    public StoreWithRating StoreWithRating { get; }
    public List<StoreRating> Ratings { get; }
    public List<IngredientWithRating> Ingredients { get; }

    public StoreWithRatingWithDetail(StoreWithRating storeWithRating, List<StoreRating> ratings, List<IngredientWithRating> ingredients)
    {
        StoreWithRating = storeWithRating;
        Ratings = ratings;
        Ingredients = ingredients;
    }
}