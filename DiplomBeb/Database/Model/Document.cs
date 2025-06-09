using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomBeb.Database.Model
{
    [Table("documents")]
    public class Document
    {
        public ulong Id { get; set; }
        public User User { get; set; } // constrained
        public string Name { get; set; }
        public string Desc { get; set; }
        public string Location { get; set; }
        public DateTime Publish_date { get; set; }
        public string Path { get; set; }
    }
}
