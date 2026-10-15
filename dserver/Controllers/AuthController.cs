using dserver.Database;
using dserver.Filters;
using dserver.Systems.Auth;
using dshared.Utils;
using Microsoft.AspNetCore.Mvc;

namespace dserver.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly MainDBContext _context;
    
    public AuthController(MainDBContext context)
    {
        _context = context;
    }
    
    // create session
    [HttpPost("signin")]
    public async Task<IActionResult> SignIn(ListElement user)
    {
        (bool state, string token) = AuthEngineServer.ServerSideSignIn(
            _context,
            HttpContext,
            user);
        if (!state)
            return BadRequest();
        
        return Ok(token);
    }
    
    // creating account for sessions
    [HttpPost("signup")]
    public async Task<IActionResult> SignUp(ListElement user)
    {
        var state = await AuthEngineServer.ServerSideSignUp(_context, HttpContext, user);
        if (!state)
            return BadRequest(state);
        return Ok(state);
    }
    
    // end session
    [AuthFilter("user")]
    [HttpGet("logout")]
    public async Task<IActionResult> LogOut()
    {
        var token = _context.Tokens.SingleOrDefault(t => t.Id == (ulong)HttpContext.Items["authed.token.id"]);
        _context.Tokens.Remove(token);
        await _context.SaveChangesAsync();
        return Ok();
    }
}