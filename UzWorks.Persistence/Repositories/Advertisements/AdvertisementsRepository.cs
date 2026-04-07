using Microsoft.EntityFrameworkCore;
using UzWorks.Core.Entities.Advertisements;
using UzWorks.Persistence.Data;

namespace UzWorks.Persistence.Repositories.Advertisements;

public class AdvertisementsRepository : GenericRepository<Advertisement>, IAdvertisementsRepository
{
    public AdvertisementsRepository(UzWorksDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Advertisement>> GetAllAsync() => 
        await _context.Advertisements.OrderBy(x => x.Title).ToArrayAsync();
}
