using Authentication.DataService.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.DataService.IConfiguration
{
    public interface IUnitOfWorks
    {
        IRefreshToknesRepository  RefreshToknes { get; }

        Task<bool> CompleteAsync();
    }
}
