using System.ComponentModel.DataAnnotations.Schema;

namespace dserver.Database.Model;

[Table("apiroles")]
public class ApiRole
{
    public ulong Id { get; set; }
    public string Name { get; set; }

    public bool IsMoreUser()
    {
        return Id > 2;
    }
}