using UzWorks.Core.DataTransferObjects.Advertisements;

namespace UzWorks.BL.Services.Advertisements;

public interface IAdvertisementService
{
    Task<AdvertisementVM> CreateAsync(AdvertisementDto advertisement);
    Task<IEnumerable<AdvertisementVM>> GetAllAsync();
    Task<AdvertisementVM> GetByIdAsync(Guid id);
    Task<AdvertisementVM> UpdateAsync(AdvertisementEM advertisement);
    Task DeleteAsync(Guid id);
}
