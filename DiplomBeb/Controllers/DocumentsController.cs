using DiplomBeb.Database;
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

        
    }
}
