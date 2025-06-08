using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using DiplomBeb.Database;
using DiplomBeb.Database.Model;
using DiplomBeb.Systems.Auth;
using DiplomBeb.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace DiplomBeb.Filters;

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