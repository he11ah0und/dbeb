using System.ComponentModel.DataAnnotations.Schema;
using DiplomBeb.Utils.AdaptiveListing;

namespace DiplomBeb.Database.Model;

[Table("document_types")]
public class DocumentType
{
    public ulong Id { get; set; }
    public string Name { get; set; }
    public string FriendlyName { get; set; }
    
    public static implicit operator ListElement(DocumentType model)
    {
        var le = new ListElement();
        le.Set("id", model.Id);
        le.Set("name", model.Name);
        le.Set("check", model.FriendlyName);
        return le;
    }
}