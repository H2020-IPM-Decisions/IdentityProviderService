namespace H2020.IPMDecisions.IDP.Core.ResourceParameters
{
    public class ApplicationUserResourceParameter : BaseResourceParameter
    {
        const int maxPageSize = 40;
        private int _pageSize = 10;
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = (value > maxPageSize) ? maxPageSize : value; }
        }
    }
}