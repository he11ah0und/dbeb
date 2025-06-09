using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using DiplomBeb.Database;
using DiplomBeb.Database.Model;
using DiplomBeb.Filters;
using Jose;
using Microsoft.EntityFrameworkCore;

namespace DiplomBeb.Systems.Auth;

public class AuthEngineServer
{
    
    
    internal static async Task<bool> Execute(
        HttpContext context,
        string theApiRole = "none",
        bool useContext = false)
    {
        bool ispublic = theApiRole == "none";
        
        (bool isLogged, ulong tokenId, string reason) = ServerSideAuthorize(
            context,
            theApiRole,
            useContext);
        
        if (!isLogged && !ispublic)
        {
            await context.Response.WriteAsync(reason);
            return false;
        }

        return true;
    }
    
    internal static async Task<bool> ServerSideSignUp(
        MainDBContext context,
        HttpContext httpContext,
        SignUpData user)
    {
        if (string.IsNullOrEmpty(user.Email)
            || (user.Password == null || user.Password.Length == 0)
            || string.IsNullOrEmpty(user.Name)
            || string.IsNullOrEmpty(user.Surname))
            return false;

        // find account by data
        var fuser = await context.Users.Where(
                (u) => (u.Email == user.Email))
            .SingleOrDefaultAsync();

        if (fuser != null)
            return false;
        
        // create new user
        var cuser = new User();
        
        // filling new user
        cuser.Name = user.Name;
        cuser.Surname = user.Surname;
        cuser.Middlename = user.Middlename;
        cuser.Email = user.Email;
        cuser.Password = user.Password;
        cuser.ApiRole = context.ApiRoles.SingleOrDefault((a) => a.Id == 1);
        cuser.WebRole = context.WebRoles.SingleOrDefault((a) => a.Id == 1);
        cuser.Score = 0;
        
        // adding user to db
        context.Users.Add(cuser);
        
        // commit changes to server
        await context.SaveChangesAsync();
        
        return true;
    }
    
    internal static (bool, string) ServerSideSignIn(
        MainDBContext context,
        HttpContext httpContext,
        SignInData user)
    {
        if (string.IsNullOrEmpty(user.Email)
            || (user.Password == null || user.Password.Length == 0))
            return (false, "");

        // find account by data
        var fuser = context.Users
            .SingleOrDefault(u => (u.Email == user.Email)
                                  && u.Password == user.Password);

        if (fuser == null)
            return (false, "");
        
        // generate token
        var now = DateTime.Now.TimeOfDay;
        var add = TimeSpan.FromHours(12);
        var tokenUser = new User() { Id = fuser.Id };
        var token = new Token { Issued = now.ToString(), Expired = now.Add(add).ToString(), User = fuser};
        
        // add to db new token for user
        context.Tokens.Add(token);
        
        // commit changes to db
        context.SaveChanges();
        
        token.User = tokenUser;

        var jwt = AuthEngineShared.EncodeToken(token);
        
        return (true, jwt);
    }
    internal static (bool, ulong, string) ServerSideAuthorize(
        HttpContext context,
        string theApiRole = "none",
        bool useContext = false)
    {
        var result = PrivateServerSideAuthorize(context, theApiRole, useContext);
        context.Items.TryAdd("authed", result.Item1);
        context.Items.TryAdd("authed.id", result.Item3);
        context.Items.TryAdd("authed.token.id", result.Item2);
        return result;
    }
    
    private static (bool, ulong, string) PrivateServerSideAuthorize(
        HttpContext context,
        string theApiRole = "none",
        bool useContext = false)
    {
        // prepare dbconect for db requesting
        var dbcontext = context.RequestServices.GetService<MainDBContext>();
        
        // get apirole by api request
        var apirole = dbcontext.ApiRoles.SingleOrDefault((ar) => ar.Name == theApiRole);
        if (apirole == null)
        {
            // not valid apirole by name
            return (false, 0, "not valid apirole by name");
        }
        
        // prepare token position
        string token;
        
        // 2: getting auth bearer from connection headers
        var vals = context.Request.Headers.Authorization.ToString();
        if (!vals.StartsWith("Bearer "))
        {
            // bearer token not found
            return (false, 0, "bearer token not found");
        }
        else
            // get as string token from headers
            token = vals.Replace("Bearer ", "");
        
        // deserialize token
        var jsont = AuthEngineShared.DecodeToken(token);
        
        // try find same token
        var thetoken = dbcontext.Tokens
            .Include(t => t.User)
            .ThenInclude(t => t.ApiRole)
            .SingleOrDefault((t =>
                t.Id == jsont.Id &&
                t.User.Id == jsont.User.Id));
        
        if (thetoken == null)
        {
            // not valid token
            return (false, 0, "not valid token");
        }
        
        // validating received token
        if (jsont.Id != thetoken.Id) // check for expiration
        {
            // not valid token
            return (false, 0, "not valid token");
        }
        
        // check apirole of user with required
        if (thetoken.User.ApiRole.Id < apirole.Id)
        {
            // apirole lower than required
            return (false, 0, "apirole lower than required");
        }
        
        return (true, thetoken.Id, thetoken.User.Id.ToString());
    }
}