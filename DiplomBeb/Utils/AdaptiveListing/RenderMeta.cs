using Microsoft.AspNetCore.Components;

namespace DiplomBeb.Utils.AdaptiveListing;

public class RenderMeta
{
    public Dictionary<string, object> Parameters { get; private set; }
    public bool Ok { get; private set; }
    public ListProp ListProp { get; private set; }

    public RenderMeta()
    {
        Ok = false;
    }
    public RenderMeta(
        ListProp listProp,
        DataResolver resolver,
        string propName,
        ListElement listElement,
        ulong index,
        Action<ObjectContainer> cce)
    {
        Parameters = [];
        ListProp = listProp;
        
        SetDataResolver(resolver);
        SetListElement(listElement);
        SetId(propName);
        SetIndex(index);
        SetCCE(cce);
        
        Ok = true;
    }

    public void Set(string id, object obj)
    {
        if (Parameters.TryAdd(id, obj))
            return;
        Parameters[id] = obj;
    }

    public void SetDataResolver(DataResolver value) 
        => Set("DataResolver", value);
    
    public void SetListElement(ListElement value) 
        => Set("ListElement", value);
    
    public void SetId(string value) 
        => Set("Id", value);
    
    public void SetIndex(ulong value) 
        => Set("Index", value);
    
    public void SetCCE(Action<ObjectContainer> value) 
        => Set("OnCallback", value);
}