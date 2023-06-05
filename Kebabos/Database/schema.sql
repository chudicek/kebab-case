-- Create enum types
CREATE TYPE store_status AS ENUM ('OPEN', 'CLOSED');
CREATE TYPE ingredient_status AS ENUM ('AVAILABLE', 'UNAVAILABLE');

-- Create tables
CREATE TABLE store (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  name VARCHAR(256) NOT NULL UNIQUE,
  status store_status NOT NULL DEFAULT 'CLOSED'
);

CREATE TABLE ingredient (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  name VARCHAR(256) NOT NULL,
  status ingredient_status NOT NULL DEFAULT 'UNAVAILABLE',
  store_id UUID NOT NULL,
  FOREIGN KEY (store_id) REFERENCES store (id),
  CONSTRAINT uc_ingredient_name_store UNIQUE (name, store_id)
);

CREATE TABLE "user" (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  name VARCHAR(256) NOT NULL UNIQUE
);

CREATE TABLE store_rating (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  rating INT CHECK (rating >= 0 AND rating <= 5),
  status_accuracy INT CHECK (rating >= 0 AND rating <= 5),
  comment TEXT,
  user_id UUID NOT NULL,
  store_id UUID NOT NULL,
  FOREIGN KEY (user_id) REFERENCES "user" (id),
  FOREIGN KEY (store_id) REFERENCES store (id)
);

CREATE TABLE ingredient_rating (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  rating INT CHECK (rating >= 0 AND rating <= 5),
  status_accuracy INT CHECK (rating >= 0 AND rating <= 5),
  comment TEXT,
  user_id UUID NOT NULL,
  ingredient_id UUID NOT NULL,
  FOREIGN KEY (user_id) REFERENCES "user" (id),
  FOREIGN KEY (ingredient_id) REFERENCES ingredient (id)
);
