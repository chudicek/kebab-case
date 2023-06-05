namespace Kebabos.Models;

public class Ingredient
{
    public Guid Id { get; }
    public Guid StoreId { get; }
    public string Name { get; }
    public IngredientStatus Status { get; }

    public Ingredient(Guid id, Guid storeId, string name, IngredientStatus status)
    {
        Id = id;
        StoreId = storeId;
        Name = name;
        Status = status;
    }
}