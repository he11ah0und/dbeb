using dserver.Systems.Auth;
using Microsoft.AspNetCore.Mvc.Filters;

namespace dserver.Filters;

public class AuthFilter : Attribute, IAsyncActionFilter
{
    // argument for apirole from api developing
    public string ApiRole { get; private set; }
    public bool UseContext { get; private set; }
    public AuthFilter(string apirole = "none", bool useContext = false)
    {
        ApiRole = apirole;
        UseContext = useContext;
    }
    
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!await AuthEngineServer.Execute(context.HttpContext, ApiRole, UseContext))
            return;
        
        // continue request
        await next();
    }
}