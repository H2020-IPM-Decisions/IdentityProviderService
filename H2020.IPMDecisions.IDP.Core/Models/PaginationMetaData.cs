namespace H2020.IPMDecisions.IDP.Core.Models
{
    public class PaginationMetaData
    {
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}