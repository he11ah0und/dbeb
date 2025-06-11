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

    public RenderMeta? GetProp(string propName, ListElement listElement, ulong index, ListView lv)
    {
        var lp = ListProps.GetValueOrDefault(
            propName,
            new ListProp(null, null));
        if (lp.IsNullable)
            return new RenderMeta();
        
        return lp.GenerateRenderMeta(this, propName, listElement, index,
            (cce) =>
                lv.OnElementCallback.InvokeAll(propName, index, listElement, cce));
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