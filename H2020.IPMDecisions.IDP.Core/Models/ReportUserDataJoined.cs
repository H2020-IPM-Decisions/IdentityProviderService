using System;

namespace H2020.IPMDecisions.IDP.Core.Models
{
    public class ReportUserDataJoined
    {
        public ReportDataFarm FarmData { get; set; }
        public ApplicationUserForReport User { get; set; }
    }

    public class ReportUserDataJoinedFlat
    {
        public string Country { get; set; }
        public string FirstCharactersUserId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime LastValidAccess { get; set; }
        public string UserType { get; set; }
        public string ModelName { get; set; }
        public string ModelId { get; set; }
    }
}