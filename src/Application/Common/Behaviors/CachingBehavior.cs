using Application.Common.Caching;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Common.Behaviors;

public sealed class CachingBehavior<TRequest, TResponse>(IMemoryCache cache)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(5);

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not ICachedQuery query)
            return await next(cancellationToken);

        if (cache.TryGetValue(query.CacheKey, out TResponse? cached) && cached is not null)
            return cached;

        var response = await next(cancellationToken);

        cache.Set(query.CacheKey, response, query.Expiration ?? DefaultExpiration);

        return response;
    }
}
