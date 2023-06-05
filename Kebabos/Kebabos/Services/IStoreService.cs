namespace Kebabos.Services.Store;

using Kebabos.Models;
using Kebabos.Contracts.Store;

public interface IStoreService
{
    Task<ServiceResult<Store>> CreateStore(StoreCreateRequest request);
    Task<ServiceResult<List<StoreWithRating>>> GetStores();
    Task<ServiceResult<StoreWithRatingWithDetail>> GetStoreById(Guid id);
    Task<ServiceResult<Store>> UpdateStoreStatus(Guid id, StoreStatusUpdateRequest request);
    Task<ServiceResult<Ingredient>> CreateIngredient(Guid storeId, IngredientCreateRequest request);
    Task<ServiceResult<Ingredient>> UpdateIngredientStatus(Guid id, IngredientStatusUpdateRequest request);
    Task<ServiceResult<StoreRating>> AddStoreRating(Guid id, CreateStoreReviewRequest request);
    Task<ServiceResult<List<IngredientWithRating>>> GetIngredients(Guid storeId);
    Task<ServiceResult<IngredientWithRatingWithDetail>> GetIngredientById(Guid id);
    Task<ServiceResult<IngredientRating>> AddIngredientReview(Guid id, CreateIngredientReviewRequest request);
}