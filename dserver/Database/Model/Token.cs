using System.ComponentModel.DataAnnotations.Schema;

namespace dserver.Database.Model;

[Table("tokens")]
public class Token
{
    public ulong Id { get; set; }
    public User User { get; set; } // constrained
    public string Issued { get; set; }
    public string Expired { get; set; }
}