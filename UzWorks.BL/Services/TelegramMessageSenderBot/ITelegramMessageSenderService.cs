using UzWorks.Core.DataTransferObjects.Jobs;
using UzWorks.Core.DataTransferObjects.TelegramMessageSenderBotDTOs;

namespace UzWorks.BL.Services.TelegramMessageSenderBot
{
    public interface ITelegramMessageSenderService
    {
        Task<bool> SendMessageAndCheckForUpdatesForJobAsync(Guid id);
        Task<bool> SendMessageAndCheckForUpdatesForWorkerAsync(Guid id);
    }
}
