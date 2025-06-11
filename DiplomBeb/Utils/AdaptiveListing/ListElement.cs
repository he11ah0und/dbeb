using System.Text.Json.Serialization;

namespace DiplomBeb.Utils.AdaptiveListing;

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

    public string Get(string key)
        => Elements.GetValueOrDefault(key, "<VALUE>");
    public string Get(string key, string defaultValue)
        => Elements.GetValueOrDefault(key, defaultValue);

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