namespace ApplicationMobilePointeuse.Models
{
    public class Students
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string StudentName
        {
            get { return $"{FirstName} {LastName}"; }
        }
    }
}