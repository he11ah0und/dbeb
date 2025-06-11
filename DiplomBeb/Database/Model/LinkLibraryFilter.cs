using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomBeb.Database.Model;

[Table("link_library_filters")]
public class LinkLibraryFilter
{
    [Column("id")]
    public ulong Id { get; set; }
    public LibraryFilter Filter { get; set; }
    public Library Library { get; set; }
}