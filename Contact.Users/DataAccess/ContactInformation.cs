namespace Contact.Users.DataAccess
{
    public class ContactInformation
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string InformationType { get; set; }
        public string InformationDetail { get; set; }

        public User User { get; set; }
    }
}
