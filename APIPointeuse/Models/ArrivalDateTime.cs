using System.ComponentModel.DataAnnotations.Schema;

namespace APIPointeuse.Models
{
    public class ArrivalDateTime
    {
        public int Id { get; set; }
        public DateTime ArrivalSavedDate { get; set; }
        public int IdStudent { get; set; }
        [ForeignKey("IdStudent")]
        public virtual Students? Students { get; set; }
        public bool IsDeleted { get; set; }
    }
}
