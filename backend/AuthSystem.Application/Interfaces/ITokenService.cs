using AuthSystem.Domain.Entities;

namespace AuthSystem.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}
