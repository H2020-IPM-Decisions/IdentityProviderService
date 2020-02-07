namespace H2020.IPMDecisions.IDP.Core.ResourceParameters
{
    public class ApplicationClientResourceParameter: BaseResourceParameter
    {

        public bool? IsEnabled { get; set; }
        const int maxPageSize = 20;
        private int _pageSize = 10;
    }
}