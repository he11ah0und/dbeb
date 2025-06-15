using DiplomBeb.Utils.AdaptiveListing;

namespace DiplomBeb.Utils;

public class Selector
{
    public string SaveProp { get; set; }
    public List<string> Values { get; private set; }
    public bool UpdateDestination { get; set; }
    
    private ListView? _sourceListView;
    public ListView? SourceListView
    {
        get => _sourceListView;
        set
        {
            _sourceListView?
                .OnElementCallback?
                .Unsubscribe("Selector");
            _sourceListView = value;
            _sourceListView?
                .OnElementCallback?
                .Subscribe("Selector", OnElementCallback);
        }
    }
    
    private ListView? _destListView;
    public ListView? DestListView
    {
        get => _destListView;
        set
        {
            _destListView?
                .OnElementCallback?
                .Unsubscribe("Selector");
            _destListView = value;
            _destListView?
                .OnElementCallback?
                .Subscribe("Selector", OnElementCallback);
        }
    }

    public Selector(ListView? sourcelistView, ListView? destListView, string saveProp)
    {
        SaveProp = saveProp;
        DestListView = destListView;
        Values = [];
        SourceListView = sourcelistView;
    }
    
    private void OnElementCallback(string propName,
        ulong index,
        ListElement listElement,
        ObjectContainer args)
    {
        var state = (bool)args.Get("value");
        if (state)
            SetValue(listElement.Get(SaveProp));
        else
            DelValue(listElement.Get(SaveProp));

        if (UpdateDestination)
        {
            DestListView?.Update();
        }
    }
    
    public void SetValue(string filter)
    {
        if (Values.Contains(filter)) return;
        Values.Add(filter);
    }
    public void DelValue(string filter) 
        => Values.Remove(filter);
    public void ClearValues()
        => Values.Clear();
}