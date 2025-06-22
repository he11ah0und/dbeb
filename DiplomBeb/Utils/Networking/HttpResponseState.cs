namespace DiplomBeb.Utils.Networking;

public class HttpResponseState
{
    public Exception Exception;
    public HttpResponseMessage Value;
    public bool IsOk { get;  set; }
    public HttpResponseState(bool isok)
    {
        IsOk = isok;
    }

    public bool ResultAsString(out string? result)
    {
        try
        {
            result = Value.Content.ReadAsStringAsync().Result;
        }
        catch
        {
            result = null;
            return false;
        }

        return true;
    }

    public bool ReadAsBytes(out byte[]? data)
    {
        try
        {
            var s = Value.Content.ReadAsStreamAsync().Result;
            var ms = new MemoryStream();
            s.CopyTo(ms);
            data = ms.ToArray();
            ms.Close();
        }
        catch
        {
            data = null;
            return false;
        }

        return true;
    }
    
    public bool ResultAsBool(out bool result)
    {
        var state = ResultAsString(out string sResult);
        state = bool.TryParse(sResult, out result);
        return state;
    }
    
    public bool ResultAsULong(out ulong result)
    {
        var state = ResultAsString(out string sResult);
        state = ulong.TryParse(sResult, out result);
        return state;
    }
    
    public bool ResultAsJson<T>(out T result)
    {
        try
        {
            result = Value.Content.ReadFromJsonAsync<T>().Result;
        }
        catch
        {
            result = default;
            return false;
        }

        return true;
    }
    
    public bool ResultAsJson(Type type, out object result)
    {
        try
        {
            result = Value.Content.ReadFromJsonAsync(type).Result;
        }
        catch
        {
            result = default;
            return false;
        }

        return true;
    }
}