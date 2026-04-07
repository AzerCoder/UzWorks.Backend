using Microsoft.AspNetCore.Mvc;
using UzWorks.BL.Services.TelegramMessageSenderBot;

namespace UzWorks.API.Controllers
{
    public class TelegramMessageSenderBotController : BaseController
    {
        private readonly ITelegramMessageSenderService _telegramMessageService;
        public TelegramMessageSenderBotController(ITelegramMessageSenderService telegramMessageSenderService)
        {
            _telegramMessageService = telegramMessageSenderService;
        }

        [HttpPost]
        public async Task<ActionResult> SendMessageToTelegramGroupsAndChannelsForJob([FromRoute]Guid jobId)
        {
           return Ok(await _telegramMessageService
                                    .SendMessageAndCheckForUpdatesForJobAsync(jobId));
        }

        [HttpPost]
        public async Task<ActionResult> SendMessageToTelegramGroupsAndChannelsForWorker([FromRoute] Guid jobId)
        {
            return Ok(await _telegramMessageService
                                     .SendMessageAndCheckForUpdatesForWorkerAsync(jobId));
        }
    }
}
