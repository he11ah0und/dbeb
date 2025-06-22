using System.Text.Json.Serialization;

namespace dshared.Utils;

public delegate void CallbackPropInElement(
    string propName,
    ulong index,
    ListElement listElement,
    ObjectContainer args);
public class ListElement
{
    [JsonPropertyName("elements")]
    public Dictionary<string, string> Elements { get; set; }

    public ListElement()
    {
        Elements = [];
    }
    public string? Get(string key)
        => Elements.GetValueOrDefault(key, null);
    public string Get(string key, string defaultValue)
        => Elements.GetValueOrDefault(key, defaultValue);

    public string? GetCombined(string divider, params string[] keys)
        => string.Join(divider, keys.Select(Get).Where(s => !string.IsNullOrEmpty(s)));

    public void Intersect(params string[] keys)
    {
        List<string> toRemove = [];
        toRemove
            .AddRange(from key in Elements where !keys.Contains(key.Key) select key.Key);
        foreach (var key in toRemove)
            Elements.Remove(key);
    }

    public void Del(string key)
    {
        Elements.Remove(key);
    }
    public void Set(string key, object? value)
    {
        if (value is null)
            return;
        Set(key, value.ToString());
    }
    public void Set(string key, string? value)
    {
        if (string.IsNullOrEmpty(value) || Elements.TryAdd(key, value))
            return;
        Elements[key] = value;
    }
}