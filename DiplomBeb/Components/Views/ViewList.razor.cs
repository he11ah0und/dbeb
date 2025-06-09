using DiplomBeb.Utils;
using DiplomBeb.Utils.AdaptiveListing;
using Microsoft.AspNetCore.Components;

namespace DiplomBeb.Components.Views;

public partial class ViewList : ComponentBase
{
    [Parameter]
    public ListView ListView { get; set; }

    public ViewList()
    {
        ListView = new ListView(null, 0, false);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        
    }
}