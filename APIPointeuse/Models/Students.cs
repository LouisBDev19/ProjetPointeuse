using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIPointeuse.Models
{
    public class Students
    {
        public int Id { get; set; }
        [MaxLength(25)]
        public string FirstName { get; set; }
        [MaxLength(25)]
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        [MaxLength(50)]
        public string? Email { get; set; }
        [MaxLength(10)]
        public string? PhoneNumber { get; set;}
        public string? Signature { get; set; }
        public int? IdSchoolclass { get; set; }
        [ForeignKey("IdSchoolclass")]
        public virtual Schoolclasses? Schoolclasses { get; set; }
        public ICollection<ArrivalDateTime>? ArrivalDateTime { get; set; }
        public bool IsDeleted { get; set; }
        [NotMapped]
        public List<Dictionary<string, DateTime?>>? UniqueDates { get; set; }
    }
}
