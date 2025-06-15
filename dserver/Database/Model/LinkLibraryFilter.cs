using System.ComponentModel.DataAnnotations.Schema;

namespace dserver.Database.Model;

[Table("link_library_filters")]
public class LinkLibraryFilter
{
    [Column("id")]
    public ulong Id { get; set; }
    public LibraryFilter LibraryFilter { get; set; }
    public Library Library { get; set; }
}