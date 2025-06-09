using DiplomBeb.Utils;
using DiplomBeb.Utils.AdaptiveListing;
using Microsoft.AspNetCore.Components;

namespace DiplomBeb.Components.Views;

public partial class ViewList : ComponentBase
{
    public ListView ListView { get; set; }

    private string _header;

    public string Header
    {
        get => _header;
        set
        {
            _header = value;
            StateHasChanged();
        }
    }
    
    
}