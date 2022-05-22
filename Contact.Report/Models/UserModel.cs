namespace Contact.Report.Models
{
    public class UserModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Company { get; set; }

        public List<ContactInformationModel> ContactInformations { get; set; }
    }

    public class ContactInformationModel
    {
        public string InformationType { get; set; }
        public string InformationDetail { get; set; }
    }
}
