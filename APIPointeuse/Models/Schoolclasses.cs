using System.ComponentModel.DataAnnotations.Schema;

namespace APIPointeuse.Models
{
    public class Schoolclasses
    {
        public int? Id { get; set; }
        public int IdCycle { get; set; }
        [ForeignKey("IdCycle")]
        public virtual Cycles? Cycles { get; set; }
        public int IdSection { get; set; }
        [ForeignKey("IdSection")]
        public virtual Sections? Sections { get; set; }
        public int IdSubsection { get; set; }
        [ForeignKey("IdSubsection")]
        public virtual Subsections? Subsections { get; set; }
        public ICollection<Students>? Students { get; set; }
        public ICollection<Periods>? Periods { get; set; }
        public bool IsDeleted { get; set; }
        [NotMapped]
        public string? SchoolclassName { get; set; }
    }
}
