using UzWorks.Core.Entities.Advertisements;

namespace UzWorks.Persistence.Repositories.Advertisements;

public interface IAdvertisementsRepository : IGenericRepository<Advertisement>
{
    Task<IEnumerable<Advertisement>> GetAllAsync();
}
