namespace demoapi.RedisClient;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
public static class RedisBuilder
{
    public static IServiceCollection AddRedisClient(this IServiceCollection services, string RedisConnStr)
    {

        var csredis = new CSRedis.CSRedisClient(RedisConnStr);
        RedisHelper.Initialization(csredis);
        services.AddSingleton<IDistributedCache>(new Microsoft.Extensions.Caching.Redis.CSRedisCache(RedisHelper.Instance));
        return services;
    }
}