using dserver.Database;
using dserver.Database.Model.SubTypes;
using dserver.Filters;
using dshared.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dserver.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ModerController : ControllerBase
{
    private readonly MainDBContext _context;

    public ModerController(MainDBContext context)
    {
        _context = context;
    }
    
    [AuthFilter("moderator")]
    [HttpGet("document")]
    public async Task<IActionResult> ReserveDocumentToModeration()
    {
        var authedId = ulong.Parse((string)HttpContext.Items["authed.id"]);

        var result = new ListElement();
        result.Set("state", false);
        result.Set("hasWork", false);
        var now  = DateTime.UtcNow;
        KeyValuePair<ulong, ReservationDocument>? pair = _context.TempReservedDocumentPerModerator
            .SingleOrDefault(p => p.Value.ModerId == authedId && p.Value.Expire < now);
        
        // assignating document to moderator
        if (pair is null)
        {
            // process old
            var old = _context.TempReservedDocumentPerModerator
                .SingleOrDefault(p => p.Value.ModerId == authedId && p.Value.Expire >= now);
            _context.TempReservedDocumentPerModerator.Remove(old.Key);
            
            // getting new
            var q1 = _context.Documents
                .IntersectBy(_context.);

            if (q1.Any())
            {
                var documents = q1.ToArray()
                    .Where(d => !_context.TempReservedDocumentPerModerator.ContainsKey(d.Id));

                if (documents.Any())
                {
                    result.Set("hasWork", true);

                    var doc = documents.First();
                    _context.TempReservedDocumentPerModerator.Add(doc.Id, new ReservationDocument(authedId));
                }
            }
        }
        else
        {
            // has active work
            result.Set("hasWork", true);
        }
        
        // processing document
        result.Set("documentId",  pair?.Key);

        result.Set("state", true);
        return Ok(result);
    }

    [AuthFilter("moderator")]
    [HttpGet("document/accept")]
    public async Task<IActionResult> AcceptDocument()
    {
        return Ok();
    }
    
    [AuthFilter("moderator")]
    [HttpGet("document/deny")]
    public async Task<IActionResult> DenyDocument()
    {
        return Ok();
    }
}