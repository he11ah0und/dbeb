using System.ComponentModel.DataAnnotations.Schema;
using DiplomBeb.Filters;
using DiplomBeb.Utils.AdaptiveListing;

namespace DiplomBeb.Database.Model
{
    [Table("users")]
    public class User : SignUpData
    {
        // main
        public ulong? Id { get; set; }
        
        public ulong? Phone { get; set; }
        public Place? Place { get; set; } 
        public ApiRole? ApiRole { get; set; }
        public WebRole? WebRole { get; set; }
        
        public List<Document> Documents { get; set; }
        
        // public
        public ulong? Score { get; set; }

        public static implicit operator ListElement(User model)
        {
            var le = new ListElement();
            le.Set("id", model.Id);
            
            le.Set("surname", model.Surname);
            le.Set("name", model.Name);
            le.Set("middlename", model.Middlename);
            
            le.Set("phone", model.Phone);
            
            le.Set("place_id", model?.Place?.Id);
            le.Set("place_name", model?.Place?.Content);
            
            le.Set("apirole_id", model?.ApiRole?.Id);
            le.Set("apirole_name", model?.ApiRole?.Name);
            
            le.Set("webrole_id", model?.WebRole?.Id);
            le.Set("webrole_name", model?.WebRole?.Name);
            
            le.Set("score", model?.Score);
            return le;
        }
    }
}
