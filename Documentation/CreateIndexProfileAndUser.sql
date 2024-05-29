-- Индекс для ускорения условия на UserId
CREATE INDEX "IX_Profiles_UserId_NotEqual"
ON "Profiles" ("UserId");

--CREATE EXTENSION IF NOT EXISTS pg_trgm;

-- GIN индекс для ускорения поиска по шаблону в FirstName
CREATE INDEX "IX_Profiles_FirstName_trigram"
ON "Profiles" USING gin ("FirstName" gin_trgm_ops);

-- GIN индекс для ускорения поиска по шаблону в LastName
CREATE INDEX "IX_Profiles_LastName_trigram"
ON "Profiles" USING gin ("LastName" gin_trgm_ops);

-- Индекс для ускорения сортировки по ProfileId
CREATE INDEX "IX_Profiles_ProfileId_Order"
ON "Profiles" ("ProfileId");
