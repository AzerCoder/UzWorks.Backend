using System.Net.Http.Json;
using UzWorks.Core.Abstract;
using UzWorks.Core.DataTransferObjects.Jobs;
using UzWorks.Core.DataTransferObjects.TelegramMessageSenderBotDTOs;
using UzWorks.Core.DataTransferObjects.Workers;
using UzWorks.Core.Entities.JobAndWork;
using UzWorks.Core.Exceptions;
using UzWorks.Persistence.Repositories.Jobs;
using UzWorks.Persistence.Repositories.Workers;

namespace UzWorks.BL.Services.TelegramMessageSenderBot
{
    public partial class TelegramMessageSenderService : ITelegramMessageSenderService
    {
        private readonly IJobsRepository _jobsRepository;
        private readonly IWorkersRepository _workersRepository;
        private readonly IMappingService _mappingService;
        private static readonly HttpClient httpClient = new HttpClient();
        private const string BotToken = "7056287484:AAHolZTaV_p2j9a5qLQhgt3hQXrtYK4uHkA";
        private const string workerPhoto = "https://lh3.googleusercontent.com/pw/AP1GczORmiWEjUKY41gzpFxbmu53k8SSSBF0ROfDxz4ol5qVrboPyimzH781EnE3hWvX8Tx5pQEQcEbLlELdjgwi3aNoPTOG-s2bUyHINYsqdwOTgIX_f3ifBJ5asz3tpeVJ7VFlWiXhOt9iiMFUeTncrPhW=w1024-h768-s-no-gm?authuser=0";
        private const string jobPhoto = "https://lh3.googleusercontent.com/pw/AP1GczNNjjOr2aGDsSfABneqXGApd_k_kjdkI7hVA3x_SSWWnzlF9tpXZxs4mvuxIbEiMhDeCQVklEIeWshdM8xjvev5DJGeXFuFJE33CJOOGosZkyK9RV5YWNoVPo3q_2Cu8LjEW-cgukh_Vu6sr9y1_pSw=w1024-h768-s-no-gm?authuser=0";
        public TelegramMessageSenderService(IJobsRepository jobsRepository, 
                                            IMappingService mappingService,
                                            IWorkersRepository workersRepository)
        {
            _jobsRepository = jobsRepository;
            _mappingService = mappingService;
            _workersRepository = workersRepository;
        }

        public async Task<bool> SendMessageAndCheckForUpdatesForJobAsync(Guid id)
        {
            var job = await _jobsRepository.GetById(id) ??
                            throw new UzWorksException($"Could not find job with id: {id}");

            var shablon = _mappingService.Map<JobVM, Job>(job);



            var getUpdatesUrl = $"https://api.telegram.org/bot{BotToken}/getUpdates";
            var updatedResponse = await httpClient.GetFromJsonAsync<Root>(getUpdatesUrl);

            if (updatedResponse?.ok == true)
            {
                await TelegramMessageSenderService
                        .SendMessageJobAsync(updatedResponse, shablon, id);

                return true;
            }
            else
                throw new UzWorksException($"Something went wrong.");

        }

        public async Task<bool> SendMessageAndCheckForUpdatesForWorkerAsync(Guid id)
        {
            var worker = await _workersRepository.GetById(id) ??
                            throw new UzWorksException($"Could not find worker with id: {id}");

            var shablon = _mappingService.Map<WorkerVM, Worker>(worker);



            var getUpdatesUrl = $"https://api.telegram.org/bot{BotToken}/getUpdates";
            var updatedResponse = await httpClient.GetFromJsonAsync<Root>(getUpdatesUrl);

            if (updatedResponse?.ok == true)
            {
                await TelegramMessageSenderService
                        .SendMessageForWorkerAsync(updatedResponse, shablon, id);

                return true;
            }
            else
                throw new UzWorksException($"Something went wrong.");

        }
    }
}
