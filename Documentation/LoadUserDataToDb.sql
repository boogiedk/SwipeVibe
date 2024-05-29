DO $$
DECLARE
    r RECORD;
BEGIN
    FOR r IN (SELECT indexname
              FROM pg_indexes
              WHERE schemaname = 'public'  -- Замените 'public' на нужную схему
                AND indexname NOT LIKE 'pg_%'
                AND indexname NOT IN (
                    SELECT conname
                    FROM pg_constraint
                    WHERE contype IN ('p', 'u')
                )
             )
    LOOP
        EXECUTE 'DROP INDEX IF EXISTS ' || r.indexname;
    END LOOP;
END $$;
