using HbysApi.Infrastructure.Repositories;
using StackExchange.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Serilog;
using Microsoft.AspNetCore.Http;

namespace HbysApi.WebApi.Endpoints
{
    // RedisEndpoints: Minimal API endpoints for Redis operations
    // RedisEndpoints: Redis işlemleri için Minimal API endpointleri
    public static class RedisEndpoints
    {
        // Registers /api/sample endpoint for Redis cache and DB access
        // Redis cache ve veritabanı erişimi için /api/sample endpointini kaydeder
        public static void MapRedisEndpoints(this WebApplication app)
        {
            app.MapGet("/api/sample", async (HttpContext httpContext) =>
            {
                // Get required services from DI
                // DI'dan gerekli servisleri al
                var repository = httpContext.RequestServices.GetRequiredService<Repository>();
                var redis = httpContext.RequestServices.GetRequiredService<IConnectionMultiplexer>();
                var configuration = httpContext.RequestServices.GetRequiredService<IConfiguration>();

                // Get query from configuration
                // Sorguyu konfigürasyondan al
                var query = configuration["Queries:PhoneBookQuery"];

                if (string.IsNullOrEmpty(query))
                {
                    // Log warning if query is missing
                    // Sorgu eksikse uyarı logla
                    Log.Warning("Query is missing in the configuration.");
                    return Results.BadRequest("Query is missing.");
                }

                var db = redis.GetDatabase();
                var cacheKey = "PhoneBookQueryCache";

                // Check cache for data
                // Veriyi cache'de kontrol et
                Log.Information("Checking cache for key: {CacheKey}", cacheKey);
                var cachedData = await db.StringGetAsync(cacheKey);
                if (!cachedData.IsNullOrEmpty)
                {
                    // Cache hit: return cached data
                    // Cache bulundu: cache'den döndür
                    Log.Information("Cache hit for key: {CacheKey}", cacheKey);
                    var deserializedData = cachedData.HasValue
                        ? System.Text.Json.JsonSerializer.Deserialize<List<object>>(cachedData!)
                        : null;
                    if (deserializedData != null)
                    {
                        return Results.Ok(deserializedData);
                    }
                }

                try
                {
                    // Cache miss: fetch from DB and cache result
                    // Cache yok: veritabanından çek ve cache'e yaz
                    Log.Information("Cache miss for key: {CacheKey}. Fetching data from database.", cacheKey);
                    var result = await repository.ExecuteQueryAsListAsync(query);
                    Log.Information("Caching data for key: {CacheKey} with a 30-second expiration.", cacheKey);
                    var serializedData = System.Text.Json.JsonSerializer.Serialize(result);
                    await db.StringSetAsync(cacheKey, serializedData, TimeSpan.FromSeconds(30));
                    Log.Information("Returning data to the client for key: {CacheKey}", cacheKey);
                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    // Log error and return problem result
                    // Hata logla ve problem sonucu döndür
                    Log.Error(ex, "An error occurred while executing the query.");
                    return Results.Problem($"Internal server error: {ex.Message}");
                }
            });
        }
    }
}
