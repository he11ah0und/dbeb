using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace dserver.Database.Model;

[Table("reserved_documents")]
public class ReservedDocument
{
    public ulong Id { get; set; }
    public Document Document { get; set; }
    public User Moder { get; set; }
    public DateTime Expire { get; set; }
}