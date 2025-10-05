using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FCG.Application.Security
{
    public interface IUserContext
    {
        string? Id { get; }
        string? Email { get; }
        string? Nome { get; }
        List<string> Roles { get; }
        IEnumerable<Claim> Claims { get; }
    }
}