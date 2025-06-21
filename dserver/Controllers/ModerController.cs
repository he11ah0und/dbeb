using System.Threading.Channels;
using dserver.Database;
using dserver.Database.Model;
using dserver.Filters;
using dserver.Services;
using dshared.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dserver.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ModerController : ControllerBase
{
    private readonly MainDBContext _context;
    private readonly Channel<ScoreParameters> _channel;
    public ModerController(MainDBContext context, Channel<ScoreParameters> channel)
    {
        _context = context;
        _channel = channel;
    }
    
    [AuthFilter("moderator")]
    [HttpGet("document")]
    public async Task<IActionResult> ReserveDocumentToModeration()
    {
        var authedId = ulong.Parse((string)HttpContext.Items["authed.id"]);
        
        if (_context.Users
                .Include(u => u.ApiRole)
                .SingleOrDefault(u => u.Id == authedId) is not User authedUser)
            return NoContent();
        
        var result = new ListElement();
        result.Set("hasWork", false);
        var now  = DateTime.UtcNow;
        var current = _context.ReservedDocuments
            .Include(p => p.Moder)
            .Include(rd => rd.Document)
            .SingleOrDefault(p => p.Moder.Id == authedId && p.Expire < now);
        
        // assignating document to moderator
        if (current is null)
        {
            // process old
            var old = _context.ReservedDocuments
                .Include(p => p.Moder)
                .SingleOrDefault(p => p.Moder.Id == authedId && p.Expire >= now);
            if (old is not null)
            {
                _context.ReservedDocuments.Remove(old);
                await _context.SaveChangesAsync();
            }

            // getting new
            var q1 = _context.Documents
                .Intersect(_context.ReservedDocuments
                    .Select(rd => rd.Document));
            if (q1.Any())
            {
                if (q1.Any())
                {
                    result.Set("hasWork", true);

                    var doc = q1.First();
                    _context.ReservedDocuments.Add(
                        new ReservedDocument()
                        {
                            Document = doc,
                            Moder = authedUser,
                            Expire = now + TimeSpan.FromMinutes(20),
                        });
                    await _context.SaveChangesAsync();
                }
            }
        }
        else
        {
            // has active work
            result.Set("hasWork", true);
        }
        
        // processing document
        result.Set("documentId", current.Document.Id);
        result.Set("documentName", current.Document.Name);
        
        return Ok(result);
    }

    [AuthFilter("moderator")]
    [HttpPost("document/accept")]
    public async Task<IActionResult> AcceptDocument([FromBody] ListElement data)
    {
        var authedId = ulong.Parse((string)HttpContext.Items["authed.id"]);
        
        if (_context.Users
                .Include(u => u.ApiRole)
                .SingleOrDefault(u => u.Id == authedId) is not User authedUser)
            return NoContent();
        
        var now  = DateTime.UtcNow;
        var current = _context.ReservedDocuments
            .Include(p => p.Moder)
            .Include(rd => rd.Document)
            .SingleOrDefault(p => p.Moder.Id == authedId && p.Expire < now);
        
        var documentTypeId = ulong.Parse(data.Get("documentTypeId", "0"));
        if (!_context.DocumentTypes.Any(t => t.Id == documentTypeId))
            return Ok(false);
        
        current.Document.DocumentType = _context.DocumentTypes.SingleOrDefault(dt => dt.Id == documentTypeId);

        _context.ModeratedDocuments.Add(new ModeratedDocument()
        {
            Document = current.Document,
            Moder = authedUser,
            DateTime = now,
            State = bool.Parse(data.Get("state", "false"))
        });
        
        _context.ReservedDocuments.Remove(current);
        await _context.SaveChangesAsync();
        
        await _channel.Writer.WriteAsync(new ScoreParameters(current.Document.Id), HttpContext.RequestAborted);
        
        return Ok(true);
    }

    [AuthFilter()]
    [HttpPost("document/accepts")]
    public async Task<IActionResult> AcceptDocument()
    {
        await _channel.Writer.WriteAsync(new ScoreParameters(10), HttpContext.RequestAborted);
        return Ok();
    }
}