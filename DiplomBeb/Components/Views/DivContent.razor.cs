using DiplomBeb.Utils.AdaptiveListing;
using Microsoft.AspNetCore.Components;

namespace DiplomBeb.Components.Views;

public partial class DivContent : ComponentBase
{
    private ListView? _listView;

    [Parameter]
    public ListView? ListView
    {
        set
        {
            _listView?.OnListUpdated.Unsubscribe("DivContent");
            _listView = value;
            _listView?.OnListUpdated.Subscribe("DivContent", StateHasChanged);
        }
    }
}