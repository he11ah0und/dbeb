using System.ComponentModel.DataAnnotations.Schema;
using dshared.Utils;

namespace dserver.Database.Model;

[Table("places")]
public class Place
{
    public ulong Id { get; set; }
    public string Content { get; set; }
    
    public static implicit operator ListElement(Place model)
    {
        var le = new ListElement();
        le.Set("id", model?.Id);
        le.Set("name", model?.Content);
        return le;
    }
}