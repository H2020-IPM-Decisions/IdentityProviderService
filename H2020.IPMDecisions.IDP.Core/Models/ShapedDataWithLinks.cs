using System.Collections.Generic;
using H2020.IPMDecisions.IDP.Core.Dtos;

namespace H2020.IPMDecisions.IDP.Core.Models
{
    public class ShapedDataWithLinks
    {
        public IEnumerable<IDictionary<string, object>> Value { get; set; }
        public IEnumerable<LinkDto> Links { get; set; }
        public PaginationMetaData PaginationMetaData { get; set; }
    }
}