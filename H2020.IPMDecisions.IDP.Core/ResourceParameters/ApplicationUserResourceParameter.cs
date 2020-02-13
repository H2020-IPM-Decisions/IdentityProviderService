namespace H2020.IPMDecisions.IDP.Core.ResourceParameters
{
    public class ApplicationUserResourceParameter : BaseResourceParameter
    {
        const int maxPageSize = 20;
        private int _pageSize = 10;
        public override int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = (value > maxPageSize) ? maxPageSize : value; }
        }
        public override string OrderBy
        {
            get => base.OrderBy;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    base.OrderBy = "Username";
                }

                base.OrderBy = value;
            }
        }
    }
}