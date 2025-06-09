using System.ComponentModel.DataAnnotations.Schema;
using DiplomBeb.Utils.AdaptiveListing;

namespace DiplomBeb.Database.Model;

[Table("library")]
public class Library
{
    public ulong Id { get; set; }
    public string Name { get; set; }
    public string Link { get; set; }
    
    public static implicit operator ListElement(Library model)
    {
        var le = new ListElement();
        le.Set("id", model.Id);

        le.Set("name", model.Name);
        
        le.Set("link", model.Link);
        return le;
    }
}