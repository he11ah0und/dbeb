using dshared.Utils;
using Microsoft.AspNetCore.Components.Rendering;

namespace DiplomBeb.Utils.AdaptiveListing;

internal delegate string NameResolveFunc(
    List<string> props);
public class ListProp
{
    internal NameResolveFunc? NameResolver { get; set; }
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
    internal ListProp(
        NameResolveFunc? nameResolver,
        Type? renderType)
    {
        NameResolver = nameResolver;
        RenderType = renderType;
    }
}