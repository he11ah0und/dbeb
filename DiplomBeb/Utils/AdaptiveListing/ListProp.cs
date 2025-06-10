namespace DiplomBeb.Utils.AdaptiveListing;

public delegate string RenderFunc(
    DataResolver resolver, 
    ListElement listElement,
    string id,
    ulong index);
public delegate string NameResolveFunc(
    List<string> props);
public class ListProp
{
    public NameResolveFunc NameResolver { get; set; }
    public RenderFunc Render { get; set; }

    public ListProp(
        NameResolveFunc nameResolver,
        RenderFunc render)
    {
        NameResolver = nameResolver;
        Render = render;
    }
}