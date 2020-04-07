using System;
using H2020.IPMDecisions.IDP.Core.Interfaces;

namespace H2020.IPMDecisions.IDP.Core.Dtos
{
    public class UserDto : IUserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
    }
}