using Microsoft.AspNetCore.Components;

namespace DiplomBeb.Utils.AdaptiveListing;

public class DataResolver
{
    public List<string> Props { get; private set; }
    public Dictionary<string, ListProp> ListProps { get; private set; }

    public DataResolver()
    {
        Props = [];
        ListProps = [];
    }

    public Dictionary<string, string> GetListProps(
        IEnumerable<string> resolvedIds,
        Dictionary<string, string> leElements)
    {
        return resolvedIds
            .Select(id => (Id: id, Value: leElements.GetValueOrDefault(id)))
            .Where(x => x.Value != null)
            .ToDictionary(x => x.Id, x => x.Value);
    }
    
    public string? ResolvePropName(string propName) 
        => ListProps.GetValueOrDefault(
                propName,
                null)
            ?.NameResolver.Invoke(Props);

    public string RenderProp(string propName, ListElement listElement, ulong index)
    {
        return ListProps.GetValueOrDefault(propName, null)
            ?.Render?.Invoke(this, listElement, propName, index);
    }

    public RenderFragment Render(string rendered)
    {
        return (b) =>
        {
            b.AddMarkupContent(0, rendered);
        };
    }

    public bool IsHave(params string[] keys)
    {
        bool isHave = true;
        foreach (var key in keys)
        {
            if (!Props.Contains(key))
                isHave = false;
        }
        return isHave;
    }
}