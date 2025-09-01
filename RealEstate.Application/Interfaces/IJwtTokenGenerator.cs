
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string Generate(ApplicationUser user, IList<string> roles);
    }
}
