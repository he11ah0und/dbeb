using System.ComponentModel.DataAnnotations.Schema;
using dshared.Utils;

namespace dserver.Database.Model;

[Table("logs")]
public class Log
{
    public ulong Id { get; set; }
    public User User { get; set; }
    public string Action { get; set; }
    public DateTime DateTime { get; set; }
    
    public static implicit operator ListElement(Log model)
    {
        var le = new ListElement();
        le.Set("id", model.Id);
        le.Set("action", model?.Action);
        le.Set("datetime", model?.DateTime.Ticks);
        
        le.Set("surname", model?.User.Surname);
        le.Set("name", model?.User.Name);
        le.Set("middlename", model?.User.Middlename);
        le.Set("webrole_name", model?.User?.WebRole?.Name);
        
        return le;
    }
}