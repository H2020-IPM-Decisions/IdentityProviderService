using System;

namespace H2020.IPMDecisions.IDP.Core.Interfaces
{
    public interface IUserDto
    {
        Guid Id { get; set; }
        string Email { get; set; }
    }
}