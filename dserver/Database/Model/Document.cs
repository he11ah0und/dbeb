using System.ComponentModel.DataAnnotations.Schema;
using dshared.Utils;

namespace dserver.Database.Model
{
    [Table("documents")]
    public class Document
    {
        public ulong Id { get; set; }
        public DocumentType? DocumentType { get; set; }
        public User User { get; set; } // constrained
        public string Name { get; set; }
        public DateTime Publish_date { get; set; }
        [Column("cost")]
        public ulong Cost { get; set; }
        public bool Moderated { get; set; }
        
        public static implicit operator ListElement(Document model)
        {
            var le = new ListElement();
            le.Set("id", model.Id);
            le.Set("document", model.Name);
            le.Set("publish_date", model.Publish_date.Ticks);
            le.Set("cost", model.Cost);
            le.Set("moderated", model.Moderated);
            return le;
        }
    }
}
