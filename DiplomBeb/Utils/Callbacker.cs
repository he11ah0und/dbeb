namespace DiplomBeb.Utils;

public class Callbacker<TDelegate>
    where TDelegate : Delegate
{
    private Dictionary<string, TDelegate> Callbacks;
    public bool InvokeOnSubscribe { get; private set; }
    public Callbacker(bool invokeOnSubscribe = true)
    {
        InvokeOnSubscribe = invokeOnSubscribe;
        Callbacks = [];
    }

    public void Subscribe(string name, TDelegate fn)
    {
        if (InvokeOnSubscribe)
            fn?.DynamicInvoke();
        if (Callbacks.ContainsKey(name))
        {
            Callbacks.Remove(name);
        }
        Callbacks.TryAdd(name, fn);
    }

    public void Unsubscribe(string name)
    {
        Callbacks.Remove(name);
    }

    public void InvokeAll(params object[] args)
    {
        var co = Callbacks.Count;
        var cbs = Callbacks.Values.ToList();
        for (var i = 0; i < co; i++)
            cbs[i]?.DynamicInvoke(args);
    }
}