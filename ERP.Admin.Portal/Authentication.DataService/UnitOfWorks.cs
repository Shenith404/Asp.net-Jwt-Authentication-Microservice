using Authentication.DataService.IConfiguration;
using Authentication.DataService.IRepository;
using Authentication.DataService.Repository;
using Microsoft.Extensions.Logging;


namespace Authentication.DataService
{
    public class UnitOfWorks : IUnitOfWorks ,IDisposable
    {

        private readonly AppDbContext _context;

        public IRefreshToknesRepository RefreshToknes { get; private set; }

        public UnitOfWorks(AppDbContext context, ILoggerFactory loggerFactory)
        {
            _context = context;
            var logger = loggerFactory.CreateLogger("logs");
            RefreshToknes =new RefreshTokenRepository(_context,logger);

        }

        public async Task<bool> CompleteAsync()
        {
            var result = await _context.SaveChangesAsync();

            return result>0;
        }

        public void Dispose() { 
            _context.Dispose();
            }
    }
}
