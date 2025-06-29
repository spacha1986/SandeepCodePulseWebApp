using Microsoft.AspNetCore.Identity;

namespace CodePulse.Api.Repositories.Interface
{
    public interface ITokenRepository
    {
        string CreateJwtToken(IdentityUser user, List<string> roles);
    }
}
