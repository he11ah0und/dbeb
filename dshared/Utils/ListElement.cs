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