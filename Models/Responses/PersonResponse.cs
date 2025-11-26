namespace AllinOne.Models.Responses
{
    public abstract class PersonResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? Age { get; set; }
    }
}
