using DiplomBeb.Database.Model;
using DiplomBeb.Utils;
using DiplomBeb.Utils.AdaptiveListing;
using DiplomBeb.Utils.Networking;

namespace DiplomBeb.Services.Client;

public class LibraryService
{
    public ListView LibraryListView { get; private set; }
    public ListView FiltersListView { get; private set; }
    public LibraryService(BackendHttpClient http)
    {
        LibraryListView = new ListView(http)
        {
            OnGet = RequestLibs,
            OnTotal = RequestLibsTotal,
            GetHeader = () => "Библиотека полезных ссылок",
            PageSwitchMode = true,
        };
        LibraryListView.DataResolver.Props.AddRange(
            ["id", "name", "link"]);

        FiltersListView = new ListView(http)
        {
            OnGet = RequestFilters,
            OnTotal = RequestFiltersTotal,
            HideFooter = true,
            HideHeading = true,
            HideTableHeader = true,
            SlimTableData = true,
        };
        FiltersListView.DataResolver.Props.AddRange(
            ["check"]);
    }
    
    private Task<HttpResponseState> RequestLibs(BackendHttpClient http, ulong skip, ulong take)
    {
        return http.NativeRequest($"Library/{skip}/{take}");
    }

    private Task<HttpResponseState> RequestLibsTotal(BackendHttpClient http)
    {
        return http.NativeRequest("Library/total");
    }
    
    private Task<HttpResponseState> RequestFilters(BackendHttpClient http, ulong skip, ulong take)
    {
        return http.NativeRequest($"Library/filters/{skip}/{take}");
    }
    
    private Task<HttpResponseState> RequestFiltersTotal(BackendHttpClient http)
    {
        return http.NativeRequest("Library/total_filters");
    }
}