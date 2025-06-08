using System.ComponentModel.DataAnnotations.Schema;
using DiplomBeb.Filters;

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
    }
}
