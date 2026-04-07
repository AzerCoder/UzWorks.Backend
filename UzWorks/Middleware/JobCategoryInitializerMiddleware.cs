using UzWorks.BL.Services.JobCategories;
using UzWorks.Core.DataTransferObjects.JobCategories;
using UzWorks.Core.Enums.JobCategories;

namespace UzWorks.API.Middleware;

public class JobCategoryInitializerMiddleware
{
    public static async Task InitializeJobCategory(IJobCategoryService jobCategoryService)
    {
        foreach (var category in JobCategoriesDictionary.Categories)
        {
            if (await jobCategoryService.IsExist(category.Key))
                continue;

            var jobCategory = new JobCategoryDto { Title = category.Key, Description = category.Value };
            await jobCategoryService.Create(jobCategory);
        }
    }
}
