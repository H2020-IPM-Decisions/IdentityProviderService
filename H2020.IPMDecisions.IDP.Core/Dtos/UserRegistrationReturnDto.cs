namespace H2020.IPMDecisions.IDP.Core.Dtos
{
    public class UserRegistrationReturnDto : UserDto
    {
        public bool EmailSentDuringRegistration { get; set; } = true;
        public bool ProfileCreatedDuringRegistration { get; set; } = true;
    }
}