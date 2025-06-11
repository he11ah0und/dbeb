using System.ComponentModel.DataAnnotations.Schema;
using DiplomBeb.Utils.AdaptiveListing;

namespace DiplomBeb.Database.Model;

[Table("library_filters")]
public class LibraryFilter
{
    [Column("id")]
    public ulong Id { get; set; }
    [Column("name")]
    public string Name { get; set; }
    [Column("friendlyName")]
    public string FriendlyName { get; set; }
    
    public static implicit operator ListElement(LibraryFilter model)
    {
        var le = new ListElement();
        le.Set("id", model.Id);
        le.Set("name", model.Name);
        le.Set("check", model.FriendlyName);
        return le;
    }
}