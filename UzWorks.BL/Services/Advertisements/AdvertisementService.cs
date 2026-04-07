using UzWorks.Core.Abstract;
using UzWorks.Core.DataTransferObjects.Advertisements;
using UzWorks.Core.Entities.Advertisements;
using UzWorks.Core.Exceptions;
using UzWorks.Persistence.Repositories.Advertisements;

namespace UzWorks.BL.Services.Advertisements;

public class AdvertisementService(
    IAdvertisementsRepository _advertisementsRepository, 
    IMappingService _mappingService,
    IEnvironmentAccessor _environmentAccessor) : IAdvertisementService
{
    public async Task<AdvertisementVM> CreateAsync(AdvertisementDto advertisement)
    {
        var advertisementVM = _mappingService.Map<AdvertisementVM, Advertisement>(
            await _advertisementsRepository.CreateAsync(
                _mappingService.Map<Advertisement, AdvertisementDto>(advertisement)));

        await _advertisementsRepository.SaveChanges();

        return advertisementVM;
    }

    public async Task<IEnumerable<AdvertisementVM>> GetAllAsync() =>
        _mappingService.Map<IEnumerable<AdvertisementVM>, IEnumerable<Advertisement>>(
            await _advertisementsRepository.GetAllAsync());

    public async Task<AdvertisementVM> GetByIdAsync(Guid id) =>
        _mappingService.Map<AdvertisementVM, Advertisement>(
            await _advertisementsRepository.GetById(id) ?? 
                throw new UzWorksException("Addvertisement didn't find with this id"));

    public async Task<AdvertisementVM> UpdateAsync(AdvertisementEM advertisementEM)
    {
        var advertisement = await _advertisementsRepository.GetById(advertisementEM.Id) ??
            throw new UzWorksException("Advertisement didn't find with this id");

        _mappingService.Map(advertisement, advertisementEM);

        advertisement.UpdateDate = DateTime.Now;
        advertisement.UpdatedBy = Guid.Parse(_environmentAccessor.GetUserId());

        _advertisementsRepository.UpdateAsync(advertisement);
        await _advertisementsRepository.SaveChanges();

        return _mappingService.Map<AdvertisementVM, Advertisement>(advertisement);
    }

    public async Task DeleteAsync(Guid id)
    {
        _advertisementsRepository.Delete(
            await _advertisementsRepository.GetById(id) ?? 
                throw new UzWorksException("Advertisement didn't find with this id"));

        await _advertisementsRepository.SaveChanges();
    }
}
