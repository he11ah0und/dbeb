using DiplomBeb.Database;
using DiplomBeb.Filters;
using DiplomBeb.Utils.AdaptiveListing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomBeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly MainDBContext _context;

        public DocumentsController(MainDBContext context)
        {
            _context = context;
        }
        
        // user api
        [AuthFilter("user")]
        [HttpGet("docs")]
        public async Task<IActionResult> Docs()
        {
            var authedId = ulong.Parse((string)HttpContext.Items["authed.id"]);
            var authedUser = _context.Users
                .SingleOrDefault(a => a.Id == authedId);
            
            if (authedUser == null)
                return NoContent();

            var docs = _context.Documents
                .Include(d => d.User)
                .Where(d => d.User.Id == authedUser.Id)
                .Cast<ListElement>()
                .ToArray();

            return Ok(docs);
        }
        
        [AuthFilter("user")]
        [HttpGet("up")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            return Ok();
        }

        [AuthFilter("user")]
        [HttpGet("down/{docId}")]
        public async Task<IActionResult> Download(ulong docId)
        {
            return Ok();
        }
        
        [AuthFilter("user")]
        [HttpPatch("doc")]
        public async Task<IActionResult> Random(long count)
        {
            return Ok(_context.Library
                .OrderBy(o => EF.Functions.Random())
                .Take((int)count));
        }
        
        // admin api
    }
}
