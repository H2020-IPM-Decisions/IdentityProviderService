namespace H2020.IPMDecisions.IDP.Core.Configuration
{
    public class InactiveUsers
    {
        public int FirstEmailMonthInactive { get; set; }
        public int SecondEmailMonthInactive { get; set; }
        public int LastEmailMonthInactive { get; set; }
        public int DeleteAccountMonthInactive { get; set; }
    }
}