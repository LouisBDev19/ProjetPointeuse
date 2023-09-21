using System.ComponentModel.DataAnnotations.Schema;

namespace APIPointeuse.Models
{
    public class Periods
    {
        public int Id { get; set; }
        public DateTime BeginningPeriod { get; set; }
        public DateTime EndingPeriod { get; set; }
        public int IdSchoolclass { get; set; }
        [ForeignKey("IdSchoolclass")]
        public virtual Schoolclasses? Schoolclasses { get; set; }
        public bool IsDeleted { get; set; }
    }
}
