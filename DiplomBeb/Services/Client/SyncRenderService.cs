namespace DiplomBeb.Services.Client;

public class SyncRenderService
{
    public ulong Max { get; set; }

    private List<ulong> _active;
    
    public Action OnComplete { get; set; }

    public SyncRenderService()
    {
        _active = [];
    }

    public void Register(ulong id)
    {
        if (!_active.Contains(id))
            _active.Add(id);

        if ((ulong)_active.Count < Max) 
            return;
        _active.Clear();
        OnComplete?.Invoke();
    }
}