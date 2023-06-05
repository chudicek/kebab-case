## User API

### Create User

Create a new user.

**Endpoint:** `/users`

**Method:** `POST`

**Request Body:**

```json
{
  "username": "bartek <3"
}
```

**Response Body:**

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "username": "bartek <3"
}
```

### Get Users

Retrieve a list of all users.

**Endpoint:** `/users`

**Method:** `GET`

**Response Body:**

```json
[
  {
    "id": "00000000-0000-0000-0000-000000000000",
    "username": "bartek <3"
  },
  {
    "id": "00000000-0000-0000-0000-000000000001",
    "username": "jane_doe"
  }
]
```

### Get User by ID

Retrieve a user by ID.

**Endpoint:** `/users/:id`

**Method:** `GET`

**Response Body:**

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "username": "bartek <3"
}
```

### Rate Store

Rate a store.

**Endpoint:** `/store/:id/rating`

**Method:** `POST`

**Path Parameters:**

- `id` - The ID of the store to rate.

**Request Body:**

```json
{
  "rating": 5,
  "status_accuracy": 5,
  "comment": "This store is great!",
  "user_id": "00000000-0000-0000-0000-000000000000"
}
```

note that user_id should be provided in some more legit way; auth required for that tho

**Response**

- `201 Created` - If the rating was created successfully.

### Rate Ingredient

Rate an ingredient.

**Endpoint:** `/store/:id/ingredient/:ingredient_id/rating`

**Method:** `POST`

**Path Parameters:**

- `id` - The ID of the store to rate.

- `ingredient_id` - The ID of the ingredient to rate.

**Request Body:**

```json
{
  "rating": 5,
  "status_accuracy": 5,
  "comment": "This ingredient is great!",
  "user_id": "00000000-0000-0000-0000-000000000000"
}
```

note that user_id should be provided in some more legit way; auth required for that tho

**Response**

- `201 Created` - If the rating was created successfully.

## Store API

### Create Store

Create a new store.

**Endpoint:** `/store`

**Method:** `POST`

**Request Body:**

```json
{
  "name": "Omega Kebabarna",
}
```

note that user_id should be provided in some more legit way; auth required for that tho

**Response**

- `201 Created` - If the store was created successfully.

- body:

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "name": "Omega Kebabarna",
  "status": 1,  // always closed on creation
}
```

<!-- note that user_id should be provided in some more legit way; auth required for that tho -->

### Get Stores

Retrieve a list of all stores.

**Endpoint:** `/store`

**Method:** `GET`

**Response Body:**

```json
[
  {
    "store": {
      "id": "00000000-0000-0000-0000-000000000000",
      "name": "Omega Kebabarna",
      "status": 0,
    },
    "ratingSum": 5,
    "statusAccuracySum": 5,
    "ratingCount": 1,
  }
  {
    "store": {
      "id": "00000000-0000-0000-0000-000000000001",
      "name": "rodinne krmitko stryca vocka",
      "status": 1,
    },
    "ratingSum": 0,
    "statusAccuracySum": 0,
    "ratingCount": 0,
  }
]
```

lol status 0 means open, 1 closed; could not be bothered to argue with dapper

### Get Store by ID (aka get detailed store)

Retrieve a store by ID.

Shows stuff as in basic get all, but also detailed ratings, ingredients, and ingredients' rating

**Endpoint:** `/store/:id`

**Method:** `GET`

**Response Body:**

```json
{
  "storeWithRating": {
    "store": {
      "id": "00000000-0000-0000-0000-000000000000",
      "name": "Omega Kebabarna",
      "status": 0,
    },
    "ratingSum": 5,
    "statusAccuracySum": 5,
    "ratingCount": 1,
  }
  "ratings": [
    {
      "id": "00000000-0000-0000-0000-000000000000",
      "storeId": "00000000-0000-0000-0000-000000000000",
      "rating": 5,
      "status_accuracy": 5,
      "comment": "This store is great!",
      "user_id": "00000000-0000-0000-0000-000000000000"
    },
    {
      "id": "00000000-0000-0000-0000-000000000001",
      "storeId": "00000000-0000-0000-0000-000000000000",
      "rating": 5,
      "status_accuracy": 5,
      "comment": "This store is great!",
      "user_id": "00000000-0000-0000-0000-000000000001"
    }
  ],
  "ingredients": [
    {
      "ingredient": {
        "id": "00000000-0000-0000-0000-000000000000",
        "storeId": "00000000-0000-0000-0000-000000000000",
        "name": "kocici ocas",
        "status": 0,
      },
      "ratingSum": 5,
      "statusAccuracySum": 5,
      "ratingCount": 1,
    },
    {
      "ingredient": {
        "id": "00000000-0000-0000-0000-000000000001",
        "storeId": "00000000-0000-0000-0000-000000000000",
        "name": "hrancle",
        "status": 1,
      },
      "ratingSum": 0,
      "statusAccuracySum": 0,
      "ratingCount": 0,
    }
  ]
}
```

### Create Ingredient

Create a new ingredient in a store.

**Endpoint:** `/store/:id/ingredient`

**Method:** `POST`

**Path Parameters:**

- `id` - The ID of the store to create the ingredient in.

**Request Body:**

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "storeId": "00000000-0000-0000-0000-000000000000",
  "name": "kocici ocas",
  "status": 1 // always unavailable on creation
}
```

### Get All Ingredients

Retrieve a list of all ingredients in a store.

**Endpoint:** `/store/:id/ingredient`

**Method:** `GET`

**Path Parameters:**

- `id` - The ID of the store to retrieve ingredients from.

**Response Body:**

```json
[
  {
    "ingredient": {
      "id": "00000000-0000-0000-0000-000000000000",
      "storeId": "00000000-0000-0000-0000-000000000000",
      "name": "kocici ocas",
      "status": 0,
    },
    "ratingSum": 5,
    "statusAccuracySum": 5,
    "ratingCount": 1,
  },
  {
    "ingredient": {
      "id": "00000000-0000-0000-0000-000000000001",
      "storeId": "00000000-0000-0000-0000-000000000000",
      "name": "hrancle",
      "status": 1,
    },
    "ratingSum": 0,
    "statusAccuracySum": 0,
    "ratingCount": 0,
  }
]
```

### Get Ingredient

Retrieve detailed information about an ingredient in a store - including the ingredient's ratings.

**Endpoint:** `/store/:id/ingredient/:ingredient_id`

**Method:** `GET`

**Path Parameters:**

- `id` - The ID of the store to retrieve. (not really used for querying)

- `ingredient_id` - The ID of the ingredient to retrieve.

**Response Body:**

```json
{
  "ingredientWithRating": {
    "ingredient": {
      "id": "00000000-0000-0000-0000-000000000000",
      "storeId": "00000000-0000-0000-0000-000000000000",
      "name": "kocici ocas",
      "status": 0,
    },
    "ratingSum": 10,
    "statusAccuracySum": 10,
    "ratingCount": 2,
  },
  "ratings": [
    {
      "id": "00000000-0000-0000-0000-000000000000",
      "ingredientId": "00000000-0000-0000-0000-000000000000",
      "userId": "00000000-0000-0000-0000-000000000000",
      "rating": 5,
      "status_accuracy": 5,
      "comment": "10/10 would pochalovat kocoura again",
    },
    {
      "id": "00000000-0000-0000-0000-000000000001",
      "ingredientId": "00000000-0000-0000-0000-000000000000",
      "userId": "00000000-0000-0000-0000-000000000001",
      "rating": 5,
      "status_accuracy": 5,
      "comment": "No idea what that was from but it was great!",
    }
  ]
}
```

### Update Store Status

Update the status of a store.

**Endpoint:** `/store/:id/status`

**Method:** `PUT`

**Path Parameters:**

- `id` - The ID of the store to update.

**Request Body:**

```json
{
  "status": "OPEN",
}
```

<!-- note that user_id should be provided in some more legit way; auth required for that tho -->

<!-- only owner of the store can update the status -->

**Response**

- `200 OK` - If the store was updated successfully.

### Update Ingredient Status

Update the status of an ingredient in a store.

**Endpoint:** `/store/:id/ingredient/:ingredient_id/status`

**Method:** `PUT`

**Path Parameters:**

- `id` - The ID of the store to update. (not really used for querying)

- `ingredient_id` - The ID of the ingredient to update.

**Request Body:**

```json
{
  "status": "AVAILABLE",
}
```

<!-- note that user_id should be provided in some more legit way; auth required for that tho -->

<!-- only owner of the store can update the status -->

**Response**

- `200 OK` - If the ingredient was updated successfully.
