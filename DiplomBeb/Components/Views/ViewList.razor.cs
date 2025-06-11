using DiplomBeb.Utils;
using DiplomBeb.Utils.AdaptiveListing;
using Microsoft.AspNetCore.Components;

namespace DiplomBeb.Components.Views;

public partial class ViewList : ComponentBase
{

    private ListView? _listView;

    [Parameter]
    public ListView? ListView
    {
        get => _listView;
        set
        {
            _listView?.OnListUpdated.Unsubscribe("listView");
            _listView = value;
            _listView?.OnListUpdated.Subscribe("listView", StateHasChanged);
        }
    }

    public ViewList()
    {
        ListView = new ListView(null, 0, false);
    }
}