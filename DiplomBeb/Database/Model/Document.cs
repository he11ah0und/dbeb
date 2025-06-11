using System.ComponentModel.DataAnnotations.Schema;
using DiplomBeb.Utils.AdaptiveListing;

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
        
        public static implicit operator ListElement(Document model)
        {
            var le = new ListElement();
            le.Set("id", model.Id);
            le.Set("name", model.Name);
            le.Set("desc", model.Desc);
            le.Set("location", model.Location);
            le.Set("publish_date", model.Publish_date.Ticks);
            return le;
        }
    }
}
