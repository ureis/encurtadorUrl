FROM postgres:16-alpine

ENV POSTGRES_DB=encurtadorurl \
    POSTGRES_USER=encurtador \
    POSTGRES_PASSWORD=encurtador123

EXPOSE 5432

HEALTHCHECK --interval=10s --timeout=5s --retries=5 --start-period=15s \
    CMD pg_isready -U "$POSTGRES_USER" -d "$POSTGRES_DB" || exit 1
