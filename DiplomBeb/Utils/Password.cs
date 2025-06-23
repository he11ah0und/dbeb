namespace DiplomBeb.Utils;

public class Password
{
    public bool Visible { get; set; }

    public Callbacker<Action> Update { get; private set; }

    public Password()
    {
        Update = new Callbacker<Action>();
    }
    
    public void Hide()
    {
        Visible = false;
        Update.InvokeAll();
    }

    public void Show()
    {
        Visible = true;
        Update.InvokeAll();
    }
}