using DiplomBeb.Database.Model;
using DiplomBeb.Utils;
using DiplomBeb.Utils.Networking;

namespace DiplomBeb.Services.Client.Components;

public class LibraryComponent
{
    public Callbacker LibraryStateChanged { get; private set; }
    public Callbacker Library4StateChanged { get; private set; }
    public Library[] Random4Libs { get; private set; }
    public Library[] Libs { get; private set; }
    public ulong TotalLibs { get; private set; }

    public LibraryComponent()
    {
        Library4StateChanged = new Callbacker();
        LibraryStateChanged = new Callbacker();
        Random4Libs = [];
        Libs = [];
    }
    
    public void RequestLibs(BackendHttpClient http, ulong page)
    {
        var res = http.NativeRequest($"Library/get30?page={page}");
        if (!res.IsOk)
            return;
        if (res.ResultAsJson<Library[]>(out var result))
            Libs = result;
        LibraryStateChanged.InvokeAll();
    }

    public void RequestLibsTotal(BackendHttpClient http)
    {
        var res = http.NativeRequest("Library/total");
        if (!res.IsOk)
            return;
        
        if (res.ResultAsULong(out var total))
            TotalLibs = total;
    }
    
    public void RequestRandom4Libs(BackendHttpClient http)
    {
        var res = http.NativeRequest("Library/get4");
        if (!res.IsOk)
        {
            return;
        }
        if (res.ResultAsJson<Library[]>(out var result))
            Random4Libs = result;
        Library4StateChanged.InvokeAll();
    }
}