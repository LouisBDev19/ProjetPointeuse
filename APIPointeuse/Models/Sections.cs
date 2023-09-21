using System.ComponentModel.DataAnnotations;

namespace APIPointeuse.Models
{
    public class Sections
    {
        public int Id {  get; set; }
        [MaxLength(25)]
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }
}
