using UzWorks.Core.Abstract;
using UzWorks.Core.DataTransferObjects.Experiences;
using UzWorks.Core.DataTransferObjects.Workers;
using UzWorks.Core.Entities.Experiences;
using UzWorks.Core.Entities.JobAndWork;
using UzWorks.Core.Exceptions;
using UzWorks.Identity.Services.Roles;
using UzWorks.Persistence.Repositories.Districts;
using UzWorks.Persistence.Repositories.JobCategories;
using UzWorks.Persistence.Repositories.Regions;
using UzWorks.Persistence.Repositories.Workers;
using UzWorks.Persistence.Repositories.Workers.Experiences;

namespace UzWorks.BL.Services.Workers;

public class WorkerService(
    IWorkersRepository _workersRepository,
    IMappingService _mappingService,
    IEnvironmentAccessor _environmentAccessor,
    IUserService _userService,
    IDistrictsRepository _districtsRepository,
    IRegionsRepository _regionsRepository,
    IJobCategoriesRepository _jobCategoriesRepository,
    IExperienceRepository _experienceRepository) : IWorkerService
{
    public async Task<WorkerVM> Create(WorkerDto workerDto)
    {
        if (workerDto.BirthDate == default)
            throw new UzWorksException("BirthDate is required.");

        if (!await _districtsRepository.IsExist(workerDto.DistrictId))
            throw new UzWorksException($"Could not find district with id: {workerDto.DistrictId}");

        if (!await _jobCategoriesRepository.IsExist(workerDto.CategoryId))
            throw new UzWorksException($"Could not find job category with id: {workerDto.CategoryId}");

        var worker = _mappingService.Map<Worker, WorkerDto>(workerDto) ??
            throw new UzWorksException("Could not map WorkerDto to Worker.");
        
        worker.CreateDate = DateTime.Now;
        worker.CreatedBy = Guid.Parse(_environmentAccessor.GetUserId());
        worker.Status = false;

        var userId = Guid.Parse(_environmentAccessor.GetUserId());

        if (_environmentAccessor.IsAdmin(userId))
        {
            worker.Status = true;
            worker.IsTop = true;   
        }

        var district = await _districtsRepository.GetById(workerDto.DistrictId)??
            throw new UzWorksException("District not found");

        var jobCategory = await _jobCategoriesRepository.GetById(workerDto.CategoryId);
        var region = await _regionsRepository.GetByDistrictId(district.Id);

        worker.District = district;
        worker.JobCategory = jobCategory;
        worker.District.Region = region;

        await _workersRepository.CreateAsync(worker);
        await _workersRepository.SaveChanges();

        var result = _mappingService.Map<WorkerVM, Worker>(worker) ??
            throw new UzWorksException("Could not map Worker to WorkerVM.");

        result.FullName = _environmentAccessor.GetFullName();

        return result;
    }

    public async Task<IEnumerable<WorkerVM>> GetAllAsync(
                        int pageNumber, int pageSize,
                        Guid? jobCategoryId, int? maxAge,
                        int? minAge, uint? maxSalary,
                        uint? minSalary, int? gender, bool? status,
                        Guid? regionId, Guid? districtId)
    {
        var workerEntities = await _workersRepository.GetAllAsync(pageNumber, pageSize, jobCategoryId,
            maxAge, minAge, maxSalary, minSalary, gender, status, regionId, districtId);

        var workers = _mappingService.Map<IEnumerable<WorkerVM>, IEnumerable<Worker>>(workerEntities).ToList();

        await FillWorkerFullNames(workers);
        await FillWorkerExperiences(workers, workerEntities);

        return workers;
    }

    public async Task<WorkerVM> GetById(Guid id)
    {
        var worker = await _workersRepository.GetById(id) ??
            throw new UzWorksException($"Could not find worker with id: {id}");

        var result = _mappingService.Map<WorkerVM, Worker>(worker);

        result.FullName = await _userService.GetUserFullName(worker.CreatedBy ??
            throw new UzWorksException("Could not be null worker created by user id."));

        if (worker.CreatedBy.HasValue)
        {
            var experiences = await _experienceRepository.GetAllByWorkerIdAsync(worker.CreatedBy.Value);
            result.Experiences = _mappingService.Map<IEnumerable<ExperienceVM>, IEnumerable<Experience>>(experiences).ToList();
        }

        return result;
    }

    public Task<int> GetCount(bool? status) =>
        _workersRepository.GetCount(status);

    public async Task<IEnumerable<WorkerVM>> GetByUserId(Guid userId)
    {
        var workerEntities = await _workersRepository.GetByUserIdAsync(userId);
        var workers = _mappingService.Map<IEnumerable<WorkerVM>, IEnumerable<Worker>>(workerEntities).ToList();

        await FillWorkerFullNames(workers);
        await FillWorkerExperiences(workers, workerEntities);

        return workers;
    }

    public async Task<IEnumerable<WorkerVM>> GetTops()
    {
        var workerEntities = await _workersRepository.GetTopsAsync();
        var workers = _mappingService.Map<IEnumerable<WorkerVM>, IEnumerable<Worker>>(workerEntities).ToList();

        await FillWorkerFullNames(workers);
        await FillWorkerExperiences(workers, workerEntities);

        return workers;
    }

    public Task<int> GetCountForFilter(Guid? jobCategoryId, int? maxAge,
                        int? minAge, uint? maxSalary,
                        uint? minSalary, int? gender, bool? status,
                        Guid? regionId, Guid? districtId) =>
        _workersRepository.GetCountForFilter(jobCategoryId,
            maxAge, minAge, maxSalary, minSalary,
            gender, status, regionId, districtId);

    public async Task<WorkerVM> Update(WorkerEM workerEM)
    {
        var worker = await _workersRepository.GetByIdWithoutExperiences(workerEM.Id) ??
            throw new UzWorksException($"Could not find worker with id: {workerEM.Id}");
        
        if (!await _districtsRepository.IsExist(workerEM.DistrictId))
            throw new UzWorksException($"Could not find district with id: {workerEM.DistrictId}");

        if (!await _jobCategoriesRepository.IsExist(workerEM.CategoryId))
            throw new UzWorksException($"Could not find job category with id: {workerEM.CategoryId}");

        if (!_environmentAccessor.IsAuthorOrSupervisor(worker.CreatedBy))
            throw new UzWorksException("You have not access for update this worker model.");

        _mappingService.Map(workerEM, worker);

        var district = await _districtsRepository.GetById(workerEM.DistrictId) ??
            throw new UzWorksException($"Could not find district with id: {workerEM.DistrictId}");

        var jobCategory = await _jobCategoriesRepository.GetById(workerEM.CategoryId);
        var region = await _regionsRepository.GetByDistrictId(district.Id);
        var userId = Guid.Parse(_environmentAccessor.GetUserId());

        worker.UpdateDate = DateTime.Now;
        worker.UpdatedBy = userId;
        worker.District = district;
        worker.JobCategory = jobCategory;
        worker.District.Region = region;

        if (!_environmentAccessor.IsAdmin(userId))
            worker.Status = false;

        _workersRepository.UpdateAsync(worker);
        await _workersRepository.SaveChanges();

        var result = _mappingService.Map<WorkerVM, Worker>(worker);

        result.FullName = await _userService.GetUserFullName(worker.CreatedBy ??
            throw new UzWorksException("Could not be null worker created by user id."));
        
        return result;
    }

    public async Task<bool?> ChangeStatus(Guid id, bool status)
    {
        var worker = await _workersRepository.GetById(id) ??
            throw new UzWorksException($"Could not find worker with id: {id}");

        if (!_environmentAccessor.IsAuthorOrSupervisor(worker.CreatedBy))
            throw new UzWorksException("You have not access for change this worker status.");

        worker.Status = status;

        _workersRepository.UpdateAsync(worker);
        await _workersRepository.SaveChanges();

        return worker.Status;
    }

    public async Task<bool> ChangeTop(Guid id, bool isTop)
    {
        var worker = await _workersRepository.GetById(id) ??
            throw new UzWorksException($"Could not find worker with id: {id}");

        if (!_environmentAccessor.IsAuthorOrSupervisor(worker.CreatedBy))
            throw new UzWorksException("You have not access for change this worker top status.");

        worker.IsTop = isTop;
        
        _workersRepository.UpdateAsync(worker);
        await _workersRepository.SaveChanges();
        
        return worker.IsTop;
    }

    public async Task<bool> Delete(Guid id)
    {
        var worker = await _workersRepository.GetById(id) ??
            throw new UzWorksException($"Could not find worker with id : {id}");

        if (!_environmentAccessor.IsAuthorOrSupervisor(worker.CreatedBy))
            throw new UzWorksException("You have not access for delete this Worker.");

        _workersRepository.Delete(worker);
        await _workersRepository.SaveChanges();

        return true;
    }

    private async Task FillWorkerFullNames(List<WorkerVM> workers)
    {
        foreach (var worker in workers)
        {
            if (worker.CreatedBy == Guid.Empty)
                continue;

            try
            {
                worker.FullName = await _userService.GetUserFullName(worker.CreatedBy);
            }
            catch
            {
                worker.FullName = string.Empty;
            }
        }
    }

    private async Task FillWorkerExperiences(List<WorkerVM> workers, IEnumerable<Worker> workerEntities)
    {
        var userIds = workerEntities
            .Where(w => w.CreatedBy.HasValue)
            .Select(w => w.CreatedBy!.Value)
            .Distinct()
            .ToList();

        if (!userIds.Any())
            return;

        var allExperiences = await _experienceRepository.GetAllByUserIdsAsync(userIds);
        var experiencesByUser = allExperiences
            .Where(e => e.CreatedBy.HasValue)
            .GroupBy(e => e.CreatedBy!.Value)
            .ToDictionary(g => g.Key, g => g.ToList());

        var workerEntityMap = workerEntities
            .Where(w => w.CreatedBy.HasValue)
            .ToDictionary(w => w.Id, w => w.CreatedBy!.Value);

        foreach (var worker in workers)
        {
            if (workerEntityMap.TryGetValue(worker.Id, out var userId) &&
                experiencesByUser.TryGetValue(userId, out var experiences))
            {
                worker.Experiences = _mappingService
                    .Map<IEnumerable<ExperienceVM>, IEnumerable<Experience>>(experiences)
                    .ToList();
            }
            else
            {
                worker.Experiences = [];
            }
        }
    }
}
