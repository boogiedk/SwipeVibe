-- GIN индекс для ускорения поиска по шаблону в FirstName
CREATE INDEX "IX_Profiles_FirstName_trigram"
ON "Profiles" USING gin ("FirstName" gin_trgm_ops);

-- GIN индекс для ускорения поиска по шаблону в LastName
CREATE INDEX "IX_Profiles_LastName_trigram"
ON "Profiles" USING gin ("LastName" gin_trgm_ops);

-- Составной индекс для полей FirstName и LastName
CREATE INDEX idx_firstname_lastname ON "Profiles" ("FirstName", "LastName");