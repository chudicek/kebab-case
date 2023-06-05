using Kebabos.Contracts.Store;
using Microsoft.AspNetCore.Mvc;
using Kebabos.Services.Store;
using Kebabos.Services;

namespace Kebabos.Controllers;

[ApiController]
[Route("store")]
public class StoreController : ControllerBase
{
    private readonly IStoreService _storeService;

    public StoreController(IStoreService storeService)
    {
        _storeService = storeService;
    }

    [HttpPost()]
    public async Task<IActionResult> CreateStore(StoreCreateRequest request)
    => (await _storeService.CreateStore(request)).process(
        store => CreatedAtAction(nameof(CreateStore), new { id = store.Id }, store),
        Common.handle
    );

    [HttpGet()]
    public async Task<IActionResult> GetStores()
    => Ok((await _storeService.GetStores()).Unwrap());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetStoreById(Guid id)
    => (await _storeService.GetStoreById(id)).process(
        Ok,
        Common.handle
    );

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStoreStatus(Guid id, StoreStatusUpdateRequest request)
    => (await _storeService.UpdateStoreStatus(id, request)).process(
        Ok,
        Common.handle
    );

    [HttpPost("{id}/ingredient")]
    public async Task<IActionResult> CreateIngredient(Guid id, IngredientCreateRequest request)
    => (await _storeService.CreateIngredient(id, request)).process(
        ingredient => CreatedAtAction(nameof(CreateIngredient), new { id = ingredient.Id }, ingredient),
        Common.handle
    );

    [HttpPut("{id}/ingredient/{ingredientId}/status")]
    public async Task<IActionResult> UpdateIngredientStatus(Guid id, Guid ingredientId, IngredientStatusUpdateRequest request)
    // yeet id
    => (await _storeService.UpdateIngredientStatus(ingredientId, request)).process(
        Ok,
        Common.handle
    );

    [HttpPost("{id}/rating")]
    public async Task<IActionResult> AddStoreReview(Guid id, CreateStoreReviewRequest request)
    => (await _storeService.AddStoreRating(id, request)).process(
        Ok,
        Common.handle
    );

    [HttpGet("{id}/ingredient")]
    public async Task<IActionResult> GetIngredients(Guid id)
    => Ok((await _storeService.GetIngredients(id)).Unwrap());

    [HttpGet("{id}/ingredient/{ingredientId}")]
    public async Task<IActionResult> GetIngredientById(Guid id, Guid ingredientId)
    => (await _storeService.GetIngredientById(ingredientId)).process(
        Ok,
        Common.handle
    );

    [HttpPost("{id}/ingredient/{ingredientId}/rating")]
    public async Task<IActionResult> AddIngredientReview(Guid id, Guid ingredientId, CreateIngredientReviewRequest request)
    // yeet store id; useless
    => (await _storeService.AddIngredientReview(ingredientId, request)).process(
        Ok,
        Common.handle
    );
}