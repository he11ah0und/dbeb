using System.ComponentModel.DataAnnotations.Schema;

namespace dserver.Database.Model;

[Table("moderated_documents")]
public class ModeratedDocument
{
    public ulong Id { get; set; }
    public Document Document { get; set; }
    public User Moder { get; set; }
    public DateTime DateTime { get; set; }
    public bool State { get; set; }

    public override string ToString()
    {
        return $"{Document.Id} {(State ? "accepted" : "denied")}";
    }
}