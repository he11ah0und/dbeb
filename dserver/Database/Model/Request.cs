using System.ComponentModel.DataAnnotations.Schema;

namespace dserver.Database.Model;

[Table("request")]
public class Request
{
    public ulong Id { get; set; }
    public User User { get; set; }
    public User Moder { get; set; }
    public string? Reason { get; set; }
}