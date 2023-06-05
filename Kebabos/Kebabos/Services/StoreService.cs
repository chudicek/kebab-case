namespace Kebabos.Services.Store;

using Kebabos.Models;
using Kebabos.Contracts.Store;
using System.Data;
using Dapper;
using Kebabos.Services;

public class StoreService : IStoreService
{
    private readonly IDbConnection _connection;

    public StoreService(IDbConnection connection)
    {
        _connection = connection;
    }

    private class DatabaseStoreResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public StoreStatus Status { get; set; }

        public Store ToEntity()
        => new Store(Id!, Name!, Status);
    }

    private async Task<Store?> GetPlainStoreById(Guid id)
    {
        var storeQuery = "SELECT id as Id, name as Name, status as Status FROM store WHERE id = @Id";
        var storeParameters = new { Id = id };
        var storeResponse = await _connection.QuerySingleOrDefaultAsync<DatabaseStoreResponse>(storeQuery, storeParameters);

        return storeResponse?.ToEntity();
    }

    private static async Task<Store?> GetPlainStoreByName(IDbConnection conn, string name)
    {
        var storeQuery = "SELECT id as Id, name as Name, status as Status FROM store WHERE name = @Name";
        var storeParameters = new { Name = name };
        var storeResponse = await conn.QuerySingleOrDefaultAsync<DatabaseStoreResponse>(storeQuery, storeParameters);

        return storeResponse?.ToEntity();
    }

    public async Task<ServiceResult<Store>> CreateStore(StoreCreateRequest request) =>
        await Common.WithConnectionOpening(_connection, async openedConnection =>
            await Common.AsTransaction(openedConnection, async _ =>
            {
                var store = await GetPlainStoreByName(openedConnection, request.StoreName);
                if (store != null) return ServiceResult<Store>.Err(ServiceError.AlreadyExists);

                var query = "INSERT INTO store (name) VALUES (@Name) RETURNING id as Id, name as Name, status as Status";
                var parameters = new { Name = request.StoreName };
                var response = await openedConnection.QuerySingleOrDefaultAsync<DatabaseStoreResponse>(query, parameters);
                return ServiceResult<Store>.FromOr(response?.ToEntity(), ServiceError.InternalError);
            })
        );

    private class DatabaseStoreWithRatingResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public StoreStatus Status { get; set; }
        public long RatingSum { get; set; }
        public long StatusAccuracySum { get; set; }
        public long RatingCount { get; set; }

        public StoreWithRating ToEntity()
        => new StoreWithRating(
            new Store(
                Id!,
                Name!,
                Status!
            ),
            RatingSum!,
            StatusAccuracySum!,
            RatingCount!
        );
    }

    public async Task<ServiceResult<List<StoreWithRating>>> GetStores()
    {
        var query = "SELECT s.id AS Id, s.name AS Name, s.status AS Status, SUM(sr.rating) AS RatingSum, SUM(sr.status_accuracy) AS StatusAccuracySum, COUNT(sr) AS RatingCount FROM store s LEFT JOIN store_rating sr ON s.id = sr.store_id GROUP BY s.id";
        var dbResponse = await _connection.QueryAsync<DatabaseStoreWithRatingResponse>(query);
        return ServiceResult<List<StoreWithRating>>.Ok(dbResponse.Select(i => i.ToEntity()).ToList());
    }

    private class DatabaseStoreRatingResponse
    {
        public Guid Id { get; set; }
        public Guid StoreId { get; set; }
        public Guid UserId { get; set; }
        public int Rating { get; set; }
        public int StatusAccuracy { get; set; }
        public string? Comment { get; set; }

        public StoreRating ToEntity()
        => new StoreRating(
            Id!,
            StoreId!,
            UserId!,
            Rating!,
            StatusAccuracy!,
            Comment // todo what about the nullability?
        );
    }

    private class DatabaseIngredientWithRatingResponse
    {
        public Guid Id { get; set; }
        public Guid StoreId { get; set; }
        public string? Name { get; set; }
        public IngredientStatus Status { get; set; }
        public long RatingSum { get; set; }
        public long StatusAccuracySum { get; set; }
        public long RatingCount { get; set; }

        public IngredientWithRating ToEntity()
        => new IngredientWithRating(
            new Ingredient(
                Id!,
                StoreId!,
                Name!,
                Status!
            ),
            RatingSum!,
            StatusAccuracySum!,
            RatingCount!
        );
    }

    public async Task<ServiceResult<StoreWithRatingWithDetail>> GetStoreById(Guid id)
    => await Common.WithConnectionOpening(_connection, async openedConnection =>
        await Common.AsTransaction(openedConnection, async _ =>
        {
            var store = await GetPlainStoreById(id);
            if (store == null) return ServiceResult<StoreWithRatingWithDetail>.Err(ServiceError.NotFound);

            var ratingsQuery = "SELECT id as Id, store_id as StoreId, user_id as UserId, rating as Rating, status_accuracy as StatusAccuracy, comment as Comment FROM store_rating WHERE store_id = @StoreId";
            var ratingsParameters = new { StoreId = id };
            var ratingsResponse = await openedConnection.QueryAsync<DatabaseStoreRatingResponse>(ratingsQuery, ratingsParameters);
            if (ratingsResponse == null) return ServiceResult<StoreWithRatingWithDetail>.Err(ServiceError.NotFound);
            var ratings = ratingsResponse.Select(i => i.ToEntity()).ToList();

            long ratingSum = ratings.Sum(r => r.Rating);
            long statusAccuracySum = ratings.Sum(r => r.StatusAccuracy);
            long ratingCount = ratings.Count();

            var ingredientsWithRating = await GetIngredients(id);
            if (ingredientsWithRating.IsErr()) return ServiceResult<StoreWithRatingWithDetail>.Err(ServiceError.NotFound);

            return ServiceResult<StoreWithRatingWithDetail>.Ok(new StoreWithRatingWithDetail(
                new StoreWithRating(
                    store,
                    ratingSum,
                    statusAccuracySum,
                    ratingCount
                ),
                ratings,
                ingredientsWithRating.Unwrap()
            ));
        })
    );

    public async Task<ServiceResult<Store>> UpdateStoreStatus(Guid id, StoreStatusUpdateRequest request)
    {
        var query = "UPDATE store SET status = @Status::store_status WHERE id = @Id RETURNING id as Id, name as Name, status as Status";
        var parameters = new { Id = id, request.Status };
        var dbResponse = await _connection.QuerySingleOrDefaultAsync<DatabaseStoreResponse>(query, parameters);
        return ServiceResult<Store>.FromOr(dbResponse?.ToEntity(), ServiceError.NotFound);
    }

    private class DatabaseIngredientResponse
    {
        public Guid Id { get; set; }
        public Guid StoreId { get; set; }
        public string? Name { get; set; }
        public IngredientStatus Status { get; set; }

        public Ingredient ToEntity()
        => new Ingredient(
            Id!,
            StoreId!,
            Name!,
            Status!
        );
    }

    private static async Task<Ingredient?> GetPlainIngredientById(IDbConnection conn, Guid id)
    {
        var query = "SELECT id as Id, store_id as StoreId, name as Name, status as Status FROM ingredient WHERE id = @Id";
        var parameters = new { Id = id };
        var response = await conn.QuerySingleOrDefaultAsync<DatabaseIngredientResponse>(query, parameters);
        return response?.ToEntity();
    }

    private static async Task<Ingredient?> GetPlainIngredientByName(IDbConnection conn, Guid storeId, string name)
    {
        var query = "SELECT id as Id, store_id as StoreId, name as Name, status as Status FROM ingredient WHERE store_id = @StoreId AND name = @Name";
        var parameters = new { StoreId = storeId, Name = name };
        var response = await conn.QuerySingleOrDefaultAsync<DatabaseIngredientResponse>(query, parameters);
        return response?.ToEntity();
    }

    public async Task<ServiceResult<Ingredient>> CreateIngredient(Guid storeId, IngredientCreateRequest request) =>
        await Common.WithConnectionOpening(_connection, async openedConnection =>
            await Common.AsTransaction(openedConnection, async _ =>
            {
                var ingredient = await GetPlainIngredientByName(openedConnection, storeId, request.IngredientName);
                if (ingredient != null) return ServiceResult<Ingredient>.Err(ServiceError.AlreadyExists);

                var query = "INSERT INTO ingredient (name, store_id) VALUES (@Name, @StoreId) RETURNING id as Id, store_id as StoreId, name as Name, status as Status";
                var parameters = new { Name = request.IngredientName, StoreId = storeId };
                var response = await openedConnection.QuerySingleOrDefaultAsync<DatabaseIngredientResponse>(query, parameters);
                return ServiceResult<Ingredient>.FromOr(response?.ToEntity(), ServiceError.InternalError);
            })
        );

    public async Task<ServiceResult<Ingredient>> UpdateIngredientStatus(Guid id, IngredientStatusUpdateRequest request)
    {
        var query = "UPDATE ingredient SET status = @Status::ingredient_status WHERE id = @Id RETURNING id as Id, store_id as StoreId, name as Name, status as Status";
        var parameters = new { Id = id, request.Status };
        var dbResponse = await _connection.QuerySingleOrDefaultAsync<DatabaseIngredientResponse>(query, parameters);
        return ServiceResult<Ingredient>.FromOr(dbResponse?.ToEntity(), ServiceError.NotFound);
    }

    public async Task<ServiceResult<StoreRating>> AddStoreRating(Guid id, CreateStoreReviewRequest request)
    {
        var query = "INSERT INTO store_rating (store_id, user_id, rating, status_accuracy, comment) VALUES (@StoreId, @UserId, @Rating, @StatusAccuracy, @Comment) returning id as Id, store_id as StoreId, user_id as UserId, rating as Rating, status_accuracy as StatusAccuracy, comment as Comment";
        var parameters = new { StoreId = id, UserId = request.UserId, Rating = request.Rating, StatusAccuracy = request.StatusAccuracy, Comment = request.Comment };
        var dbResponse = await _connection.QuerySingleOrDefaultAsync<DatabaseStoreRatingResponse>(query, parameters);
        return ServiceResult<StoreRating>.FromOr(dbResponse?.ToEntity(), ServiceError.InternalError);
    }

    public async Task<ServiceResult<List<IngredientWithRating>>> GetIngredients(Guid storeId)
    {
        var query = "SELECT i.id as Id, store_id as StoreId, i.name as Name, i.status as Status, SUM(ir.rating) as RatingSum, Sum(ir.status_accuracy) as StatusAccuracySum, COUNT(ir) as RatingCount FROM ingredient i LEFT JOIN ingredient_rating ir ON i.id = ir.ingredient_id WHERE i.store_id = @StoreId GROUP BY i.id";
        var parameters = new { StoreId = storeId };
        var dbResponse = await _connection.QueryAsync<DatabaseIngredientWithRatingResponse>(query, parameters);
        return ServiceResult<List<IngredientWithRating>>.Ok(dbResponse.Select(i => i.ToEntity()).ToList());
    }

    private class DatabaseIngredientRatingResponse
    {
        public Guid Id { get; set; }
        public Guid IngredientId { get; set; }
        public Guid UserId { get; set; }
        public int Rating { get; set; }
        public int StatusAccuracy { get; set; }
        public string? Comment { get; set; }

        public IngredientRating ToEntity()
        => new IngredientRating(
            Id!,
            IngredientId!,
            UserId!,
            Rating,
            StatusAccuracy,
            Comment
        );
    }

    public async Task<ServiceResult<IngredientWithRatingWithDetail>> GetIngredientById(Guid id)
    => await Common.WithConnectionOpening(_connection, async openedConnection =>
        await Common.AsTransaction(openedConnection, async _ =>
        {
            var ingredientQuery = "SELECT id as Id, name as Name, status as Status FROM ingredient WHERE id = @Id";
            var ingredientParameters = new { Id = id };
            var ingredientResponse = await openedConnection.QuerySingleOrDefaultAsync<DatabaseIngredientResponse>(ingredientQuery, ingredientParameters);
            if (ingredientResponse == null) return ServiceResult<IngredientWithRatingWithDetail>.Err(ServiceError.NotFound);
            var ingredient = ingredientResponse.ToEntity()!;

            var ratingsQuery = "SELECT id as Id, ingredient_id as IngredientId, user_id as UserId, rating as Rating, status_accuracy as StatusAccuracy, comment as Comment FROM ingredient_rating WHERE ingredient_id = @IngredientId";
            var ratingsParameters = new { IngredientId = id };
            var ratingsResponse = await openedConnection.QueryAsync<DatabaseIngredientRatingResponse>(ratingsQuery, ratingsParameters);
            if (ratingsResponse == null) return ServiceResult<IngredientWithRatingWithDetail>.Err(ServiceError.NotFound);
            var ratings = ratingsResponse.Select(i => i.ToEntity()).ToList();

            long ratingSum = ratings.Sum(r => r.Rating);
            long statusAccuracySum = ratings.Sum(r => r.StatusAccuracy);
            long ratingCount = ratings.Count();

            return ServiceResult<IngredientWithRatingWithDetail>.Ok(new IngredientWithRatingWithDetail(
                new IngredientWithRating(
                    ingredient,
                    ratingSum,
                    statusAccuracySum,
                    ratingCount
                ),
                ratings
            ));
        }));

    public async Task<ServiceResult<IngredientRating>> AddIngredientReview(Guid id, CreateIngredientReviewRequest request)
    {
        var query = "INSERT INTO ingredient_rating (ingredient_id, user_id, rating, status_accuracy, comment) VALUES (@IngredientId, @UserId, @Rating, @StatusAccuracy, @Comment) returning id as Id, ingredient_id as IngredientId, user_id as UserId, rating as Rating, status_accuracy as StatusAccuracy, comment as Comment";
        var parameters = new { IngredientId = id, UserId = request.UserId, Rating = request.Rating, StatusAccuracy = request.StatusAccuracy, Comment = request.Comment };
        var response = await _connection.QuerySingleOrDefaultAsync<DatabaseIngredientRatingResponse>(query, parameters);
        return ServiceResult<IngredientRating>.FromOr(response?.ToEntity(), ServiceError.InternalError);
    }
}