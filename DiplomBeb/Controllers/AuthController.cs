using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using DiplomBeb.Database;
using DiplomBeb.Database.Model;
using DiplomBeb.Filters;
using DiplomBeb.Systems.Auth;
using Jose;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomBeb.Controllers;

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
    public async Task<IActionResult> SignIn(SignInData user)
    {
        (bool state, string token) = await AuthEngineServer.ServerSideSignIn(
            _context,
            HttpContext,
            user);
        if (!state)
            return BadRequest();
        
        return Ok(token);
    }
    
    // creating account for sessions
    [HttpPost("signup")]
    public async Task<IActionResult> SignUp(SignUpData user)
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
        _context.SaveChangesAsync();
        return Ok();
    }
    
    [AuthFilter("user")]
    [HttpGet("validate")]
    public async Task<IActionResult> Validate()
    {
        return Ok(HttpContext.Items["authed"]);
    }
}