namespace DiplomBeb.Utils;

public class ObjectContainer
{
    public Dictionary<string, object> Objects { get; set; }

    public ObjectContainer()
    {
        Objects = [];
    }
    
    public object? Get(string key)
        => Objects.GetValueOrDefault(key, null);
    public object Get(string key, object defaultValue)
        => Objects.GetValueOrDefault(key, defaultValue);
    
    public void Set(string key, object? value)
    {
        if (value is null || Objects.TryAdd(key, value))
            return;
        Objects[key] = value;
    }
}