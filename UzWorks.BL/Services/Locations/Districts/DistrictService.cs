using UzWorks.Core.Abstract;
using UzWorks.Core.DataTransferObjects.Location.Districts;
using UzWorks.Core.Entities.Location;
using UzWorks.Core.Exceptions;
using UzWorks.Persistence.Repositories.Districts;

namespace UzWorks.BL.Services.Locations.Districts;

public class DistrictService(
    IDistrictsRepository _districtsRepository,
    IMappingService _mappingService) 
        : IDistrictService
{
    public async Task<DistrictVM> Create(DistrictDto districtDto)
    {
        if (districtDto == null)
            throw new UzWorksException($"District Dto can not be null.");

        var district = new District(districtDto.Name, districtDto.RegionId);
        
        await _districtsRepository.CreateAsync(district);
        await _districtsRepository.SaveChanges();

        return _mappingService.Map<DistrictVM,District>(district);
    }

    public async Task<IEnumerable<DistrictVM>> GetAllAsync() =>
        _mappingService.Map<IEnumerable<DistrictVM>, IEnumerable<District>>(
            await _districtsRepository.GetAllAsync());

    public async Task<DistrictVM> GetById(Guid id)
    {
        var district = await _districtsRepository.GetById(id) ??
            throw new UzWorksException($"Could not find District with Id: {id}");

        return _mappingService.Map<DistrictVM,District>(district);
    }

    public async Task<IEnumerable<DistrictVM>> GetByRegionId(Guid regionId)
    {
        var districts = await _districtsRepository.GetByRegionIdAsync(regionId) ??
            throw new UzWorksException($"Could not find District with this region id: {regionId}");

        return _mappingService.Map<IEnumerable<DistrictVM>, IEnumerable<District>>(districts);
    }

    public Task<bool> IsExist(string districtName) =>
        _districtsRepository.IsExist(districtName);

    public Task<bool> IsExist(Guid districtId) =>
        _districtsRepository.IsExist(districtId);

    public async Task<DistrictVM> Update(DistrictEM districtEM)
    {
        var district = await _districtsRepository.GetById(districtEM.Id)??
            throw new UzWorksException($"Could not find District with Id: {districtEM.Id}");

        _mappingService.Map(districtEM, district);

        _districtsRepository.UpdateAsync(district);
        await _districtsRepository.SaveChanges();

        return _mappingService.Map<DistrictVM,District>(district);
    }

    public async Task<bool> Delete(Guid id)
    {
        var district = await _districtsRepository.GetById(id) ??
            throw new UzWorksException($"Could not find District with id: {id}");

        _districtsRepository.Delete(district);
        await _districtsRepository.SaveChanges();

        return true;
    }
}
