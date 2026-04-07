using UzWorks.BL.Services.Jobs;
using UzWorks.BL.Services.Workers;
using UzWorks.Core.Abstract;
using UzWorks.Core.DataTransferObjects.JobCategories;
using UzWorks.Core.Entities.JobAndWork;
using UzWorks.Core.Exceptions;
using UzWorks.Persistence.Repositories.JobCategories;

namespace UzWorks.BL.Services.JobCategories;

public class JobCategoryService(
    IJobCategoriesRepository _repository,
    IMappingService _mappingService,
    IJobService _jobService,
    IWorkerService _workerService) 
        : IJobCategoryService
{
    public async Task<JobCategoryVM> Create(JobCategoryDto jobCategoryDto)
    {
        if (jobCategoryDto == null)
            throw new UzWorksException($"Job Category Dto can not be null.");

        var jobCategory = new JobCategory(jobCategoryDto.Title, jobCategoryDto.Description);

        await _repository.CreateAsync(jobCategory);
        await _repository.SaveChanges();

        var categoryVM = _mappingService.Map<JobCategoryVM, JobCategory>(jobCategory);

        categoryVM.CountOfWorkers = await _workerService.GetCountForFilter(
                categoryVM.Id, null, null, null, null, null, null, null, null);

        categoryVM.CountOfJobs = await _jobService.GetGountForFilter(
            categoryVM.Id, null, null, null, null, null, null, null, null);

        return categoryVM;
    }

    public async Task<IEnumerable<JobCategoryVM>> GetAllAsync() 
    {
        var categories = _mappingService.Map<IEnumerable<JobCategoryVM>, IEnumerable<JobCategory>>(await _repository.GetAllAsync());

        foreach (var category in categories) 
        {
            category.CountOfWorkers = await _workerService.GetCountForFilter(
                category.Id, null, null, null, null, null, null, null, null);

            category.CountOfJobs = await _jobService.GetGountForFilter(
                category.Id, null, null, null, null, null, null, null, null);
        }

        return categories;
    }

    public async Task<JobCategoryVM> GetById(Guid id)
    {
        var jobCategory = await _repository.GetById(id) ??
            throw new UzWorksException($"Could not find Job Category with id: {id}");
        
        var categoryVM = _mappingService.Map<JobCategoryVM, JobCategory>(jobCategory);

        categoryVM.CountOfWorkers = await _workerService.GetCountForFilter(
                categoryVM.Id, null, null, null, null, null, null, null, null);

        categoryVM.CountOfJobs = await _jobService.GetGountForFilter(
            categoryVM.Id, null, null, null, null, null, null, null, null);

        return categoryVM;
    }

    public async Task<bool> IsExist(string jobCategoryName) =>
        await _repository.IsExist(jobCategoryName);

    public async Task<bool> IsExist(Guid id) =>
        await _repository.IsExist(id);

    public async Task<JobCategoryVM> Update(JobCategoryEM jobCategoryEM)
    {
        var jobCategory = await _repository.GetById(jobCategoryEM.Id) ?? 
            throw new UzWorksException($"Could not find JobCategory with Id: {jobCategoryEM.Id}");
        
        _mappingService.Map(jobCategoryEM, jobCategory);
        
        _repository.UpdateAsync(jobCategory);
        await _repository.SaveChanges();

        var categoryVM = _mappingService.Map<JobCategoryVM, JobCategory>(jobCategory);

        categoryVM.CountOfWorkers = await _workerService.GetCountForFilter(
                categoryVM.Id, null, null, null, null, null, null, null, null);

        categoryVM.CountOfJobs = await _jobService.GetGountForFilter(
            categoryVM.Id, null, null, null, null, null, null, null, null);

        return categoryVM;
    }

    public async Task<bool> Delete(Guid Id)
    {
        var jobCategory = await _repository.GetById(Id) ??
            throw new UzWorksException($"Could not find JobCategory with {Id}");
        
        _repository.Delete(jobCategory);
        await _repository.SaveChanges();

        return true;
    }
}
