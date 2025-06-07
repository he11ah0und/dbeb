using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomBeb.Database.Model;

[Table("webroles")]
public class WebRole
{
    public ulong Id { get; set; }
    public string Name { get; set; }
}