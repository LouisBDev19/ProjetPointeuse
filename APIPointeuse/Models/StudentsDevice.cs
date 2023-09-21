using System.ComponentModel.DataAnnotations.Schema;

namespace APIPointeuse.Models
{
    public class StudentsDevice
    {
        public int Id { get; set; }
        public int IdStudent { get; set; }
        [ForeignKey("IdStudent")]
        public virtual Students? Students { get; set; }
        public string System { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
