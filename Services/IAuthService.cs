using EventsAPI.Models;

namespace EventsAPI.Services
{
    public interface IAuthService
    {
        Task<bool> AssignRoles(string userName, IEnumerable<string> roles);
        Task<string> GenerateToken(User user);
        Task<bool> Login(User user);
        Task<bool> RegisterUser(User user);
        Task<string> GetUserId(User user);
    }
}
