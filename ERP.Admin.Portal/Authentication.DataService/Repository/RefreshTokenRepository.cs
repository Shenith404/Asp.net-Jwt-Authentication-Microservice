using Authentication.Core.Entity;
using Authentication.DataService.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.DataService.Repository
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshToknesRepository
    {
        public RefreshTokenRepository(AppDbContext context, ILogger logger) : base(context, logger)
        {
        }

        public override async Task<IEnumerable<RefreshToken>> GetAll()
        {
            try
            {
                return await dbSet.Where(x=>x.Status==1)
                    .AsNoTracking()
                    .ToListAsync();
            }catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} All mothod has generated Error", typeof(RefreshTokenRepository));

                return new List<RefreshToken>();
            }
        }

        public async Task<RefreshToken ?> GetByRefreshToken(string refreshToken)
        {
            try
            {
                return await dbSet.Where(x => x.Token.ToLower() == refreshToken.ToLower())
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} All mothod has generated Error", typeof(RefreshTokenRepository));

                return null;
            }
        }

        public async Task<bool> MarkRefreshTokenAsUser(RefreshToken refreshToken)
        {
            try
            {
                var result = await dbSet.Where(x => x.Id == refreshToken.Id)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();


                if(result == null) return false;

                result.IsUsed = refreshToken.IsUsed;

                return true;


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} All mothod has generated Error", typeof(RefreshTokenRepository));

                return false;
            }
        }
    }
}
