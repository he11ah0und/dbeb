using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomBeb.Database.Model;

[Table("document_types")]
public class DocumentType
{
    public ulong Id { get; set; }
    public string Name { get; set; }
    public string FriendlyName { get; set; }
}