namespace H2020.IPMDecisions.IDP.Core.ResourceParameters
{
    public class RefreshTokenResourceParameter
    {
        const int maxPageSize = 20;
        private int _pageSize = 10;
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = (value > maxPageSize) ? maxPageSize : value; }
        }
        public int PageNumber { get; set; } = 1;
        public string Fields { get; set; }
    }
}