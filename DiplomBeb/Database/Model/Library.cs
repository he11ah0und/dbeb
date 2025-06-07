using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomBeb.Database.Model;

[Table("library")]
public class Library
{
    public ulong Id { get; set; }
    public string Name { get; set; }
    public string Link { get; set; }
}