using System;

namespace H2020.IPMDecisions.IDP.API.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RequiredClientHeaderAttribute : Attribute
    {
        public RequiredClientHeaderAttribute()
        {
        }        
    }
}