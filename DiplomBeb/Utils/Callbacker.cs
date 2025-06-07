namespace DiplomBeb.Utils;

public class Callbacker
{
    private Dictionary<string, Action> Callbacks;
    public bool InvokeOnSubscribe { get; private set; }
    public Callbacker(bool invokeOnSubscribe = true)
    {
        InvokeOnSubscribe = invokeOnSubscribe;
        Callbacks = [];
    }

    public void Subscribe(string name, Action fn)
    {
        if (InvokeOnSubscribe)
            fn?.Invoke();
        if (!Callbacks.TryAdd(name, fn))
            return;
        
    }

    public void Unsubscribe(string name)
    {
        Callbacks.Remove(name);
    }

    public void InvokeAll()
    {
        var co = Callbacks.Count;
        var cbs = Callbacks.Values.ToList();
        for (var i = 0; i < co; i++)
            cbs[i]?.Invoke();
    }
}