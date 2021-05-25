using System;

namespace H2020.IPMDecisions.IDP.Core.Models
{
    public class InactiveUserEmail
    {
        public string ToAddress { get; set; }
        public int InactiveMonths { get; set; }
        public string AccountDeletionDate { get; set; }
    }
}