using Microsoft.AspNetCore.Components;

namespace DiplomBeb.Services.Client;

public class SvgCache
{
    public Dictionary<string, CacheData> CachedSvgs { get; private set; }
    public Dictionary<CacheId, RenderFragment> CachedInstances { get; private set; }
    public SvgCache()
    {
        CachedSvgs = [];
        CachedInstances = [];
    }

    public bool GetCachedInstance(CacheId? cache, out RenderFragment renderFragment)
    {
        if (cache is null)
        {
            renderFragment = null;
            return false;
        }
        return CachedInstances.TryGetValue(cache, out renderFragment);
    }

    public bool SaveCachedSvg(string source, CacheData cacheData)
    {
        return CachedSvgs.TryAdd(source, cacheData);
    }
    public bool SaveCachedInstance(CacheId? cache, RenderFragment renderFragment)
    {
        return CachedInstances.TryAdd(cache, renderFragment);
    }
}
public record CacheId(string Class, string Source);
public record CacheData(RenderFragment rf, SvgParameters sp);

public class SvgParameters
{
    public SvgElement Root { get; set; }
}

public class SvgElement
{
    public string TagName { get; set; }
    public Dictionary<string, string> Attributes { get; set; } = new();
    public List<SvgElement> Children { get; set; } = new();
}