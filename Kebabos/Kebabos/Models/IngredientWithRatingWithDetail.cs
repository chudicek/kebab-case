namespace Kebabos.Models;
public class IngredientWithRatingWithDetail
{
    public IngredientWithRating IngredientWithRating { get; }
    public List<IngredientRating> Ratings { get; }

    public IngredientWithRatingWithDetail(IngredientWithRating ingredientWithRating, List<IngredientRating> ratings)
    {
        IngredientWithRating = ingredientWithRating;
        Ratings = ratings;
    }
}