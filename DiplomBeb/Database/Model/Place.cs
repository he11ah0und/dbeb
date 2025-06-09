using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomBeb.Database.Model;

[Table("places")]
public class Place
{
    public ulong Id { get; set; }
    public string Content { get; set; }
}