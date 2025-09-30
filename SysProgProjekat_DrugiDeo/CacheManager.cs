using System;
using System.Collections.Concurrent;

public class CacheManager
{
    private readonly ConcurrentDictionary<string, byte[]> cache = new ConcurrentDictionary<string, byte[]>();

    public bool TryGetValue(string key, out byte[] value)
    {
        return cache.TryGetValue(key, out value);
    }

    public void Add(string key, byte[] value)
    {
        cache.TryAdd(key, value);
    }
}