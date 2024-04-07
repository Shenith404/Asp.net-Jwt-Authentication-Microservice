using Authentication.Core.Entity;

namespace Authentication.DataService.IRepository
{
    public interface IRefreshToknesRepository : IGenericRepository<RefreshToken>
    {
        Task<RefreshToken> GetByRefreshToken(string refreshToken);

        Task<bool> MarkRefreshTokenAsUser(RefreshToken refreshToken);
    }
}
