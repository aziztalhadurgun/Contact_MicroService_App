namespace Contact.Users.DataAccess
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Company { get; set; }

        public virtual ICollection<ContactInformation> ContactInformations { get; set; }

    }
}
