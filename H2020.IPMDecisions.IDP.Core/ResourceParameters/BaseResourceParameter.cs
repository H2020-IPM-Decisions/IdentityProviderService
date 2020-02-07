namespace H2020.IPMDecisions.IDP.Core.ResourceParameters
{
    public abstract class BaseResourceParameter
    {
        public string SearchQuery { get; set; }       
        public int PageNumber { get; set; } = 1;
        
    }
}