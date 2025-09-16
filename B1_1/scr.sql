SELECT * FROM RECORDS;

DROP TABLE records;

CREATE TABLE IF NOT EXISTS records (
    id SERIAL PRIMARY KEY,
    date_field DATE NOT NULL,
    english_string VARCHAR(10) NOT NULL,
    russian_string VARCHAR(10) NOT NULL,
    positive_integer_value INT NOT NULL,
    double_value NUMERIC(20,8) NOT NULL
);

CREATE OR REPLACE FUNCTION calculate_sum_and_median()
RETURNS TABLE(sum_int bigint, median_double numeric(20,8)) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        SUM(positive_integer_value)::bigint AS sum_int,
        PERCENTILE_CONT(0.5) WITHIN GROUP (ORDER BY double_value)::numeric(20,8) AS median_double
    FROM records;
END;
$$ LANGUAGE plpgsql;

SELECT * FROM calculate_sum_and_median();
