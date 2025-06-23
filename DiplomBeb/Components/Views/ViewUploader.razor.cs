using DiplomBeb.Utils;
using Microsoft.AspNetCore.Components;

namespace DiplomBeb.Components.Views;

public partial class ViewUploader : ComponentBase
{
    private Uploader? _uploader;

    [Parameter]
    public Uploader? Uploader
    {
        set
        {
            _uploader?.OnUploaded.Unsubscribe("ViewUploader");
            if (_uploader != null)
                _uploader.OnShow = null;
            _uploader = value;
            _uploader?.OnUploaded.Subscribe("ViewUploader", OnUploaded);
            if (_uploader != null)
                _uploader.OnShow = OnShowUploader;
        }
    }

    private async Task OnUploaded()
    {
        if (_uploader == null)
            return;
        _uploader.DocumentName = "";
        _uploader?.SetDefaultFileData();
    }
    
    public bool Show { get; private set; }

    private void OnShowUploader()
    {
        Show = true;
        StateHasChanged();
    }
    private void HideUploader()
    {
        Show = false;
        StateHasChanged();
    }
}