using DiplomBeb.Utils.AdaptiveListing;
using Microsoft.AspNetCore.Components;

namespace DiplomBeb.Components.Views;

public partial class TableContent : ComponentBase
{
    [Parameter]
    public ListView? ListView { get; set; }
}