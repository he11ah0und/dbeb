using Microsoft.AspNetCore.Components;

namespace DiplomBeb.Components.Pages;

public partial class Error404 : ComponentBase
{
    [Parameter]
    public string ECode { get; set; }
    [Inject]
    private NavigationManager Navigation { get; set; }
    private void ToMain()
    {
        Navigation.NavigateTo($"/");
    }
}