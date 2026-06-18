using Application.Common.Behaviors;
using Application.Common.Caching;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Shouldly;

namespace Application.UnitTests;

public class CachingBehaviorTests
{
    private sealed record CachedQuery(string CacheKey, TimeSpan? Expiration) : IRequest<string>, ICachedQuery;

    private sealed record PlainRequest : IRequest<string>;

    [Fact]
    public async Task Returns_cached_value_without_invoking_next_on_a_hit()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        cache.Set("key", "cached");
        var behavior = new CachingBehavior<CachedQuery, string>(cache);

        var nextInvoked = false;
        RequestHandlerDelegate<string> next = _ =>
        {
            nextInvoked = true;
            return Task.FromResult("fresh");
        };

        var result = await behavior.Handle(new CachedQuery("key", null), next, CancellationToken.None);

        result.ShouldBe("cached");
        nextInvoked.ShouldBeFalse();
    }

    [Fact]
    public async Task Invokes_next_and_stores_the_result_on_a_miss()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var behavior = new CachingBehavior<CachedQuery, string>(cache);
        RequestHandlerDelegate<string> next = _ => Task.FromResult("fresh");

        var result = await behavior.Handle(new CachedQuery("key", null), next, CancellationToken.None);

        result.ShouldBe("fresh");
        cache.TryGetValue("key", out string? stored).ShouldBeTrue();
        stored.ShouldBe("fresh");
    }

    [Fact]
    public async Task Bypasses_the_cache_for_requests_that_are_not_cacheable()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var behavior = new CachingBehavior<PlainRequest, string>(cache);

        var nextInvoked = false;
        RequestHandlerDelegate<string> next = _ =>
        {
            nextInvoked = true;
            return Task.FromResult("fresh");
        };

        var result = await behavior.Handle(new PlainRequest(), next, CancellationToken.None);

        result.ShouldBe("fresh");
        nextInvoked.ShouldBeTrue();
    }
}
