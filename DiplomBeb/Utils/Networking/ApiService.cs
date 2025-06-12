using Microsoft.Extensions.Options;

namespace DiplomBeb.Utils.Networking;

public class ApiService
{
    public ApiOptions Options { get; set; }
    public ApiService(IOptions<ApiOptions> options)
    {
        Options = options.Value;
    }
}