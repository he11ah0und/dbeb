using Microsoft.AspNetCore.Components.Rendering;

namespace DiplomBeb.Utils.AdaptiveListing;

public delegate string NameResolveFunc(
    List<string> props);
public class ListProp
{
    public NameResolveFunc? NameResolver { get; set; }
    public Type? RenderType { get; set; }
    public bool IsNullable => NameResolver is null || RenderType is null;
    
    public RenderMeta GenerateRenderMeta(
        DataResolver resolver,
        string propName,
        ListElement listElement,
        ulong index,
        Action<ObjectContainer> cce)
    {
        return new RenderMeta(
            this,
            resolver,
            propName,
            listElement,
            index,
            cce);
    }
    public ListProp(
        NameResolveFunc? nameResolver,
        Type? renderType)
    {
        NameResolver = nameResolver;
        RenderType = renderType;
    }
}