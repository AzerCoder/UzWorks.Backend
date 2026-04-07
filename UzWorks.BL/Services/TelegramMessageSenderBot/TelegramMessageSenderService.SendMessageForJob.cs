using System.Text;
using System.Text.Json;
using UzWorks.Core.DataTransferObjects.Jobs;
using UzWorks.Core.DataTransferObjects.TelegramMessageSenderBotDTOs;

namespace UzWorks.BL.Services.TelegramMessageSenderBot
{
    public partial class TelegramMessageSenderService
    {
        public static async Task SendMessageJobAsync(Root updatedResponse,
                                                     JobVM shablon,
                                                     Guid id)
        {
            var adminChatIds = new List<long>();
            foreach (var update in updatedResponse.result)
            {
                var chat = update.my_chat_member?.chat ?? update.channel_post?.chat;

                if (chat == null || !new[] { "group", "supergroup", "channel" }
                            .Contains(chat.type) || adminChatIds.Contains(chat.id))
                    continue;

                adminChatIds.Add(chat.id);
                var message = TelegramMessageSenderService.GenerateTelegramMessage(shablon);

                var sendPhotoUrl = $"https://api.telegram.org/bot{BotToken}/sendPhoto";

                var payload = new
                {
                    chat_id = chat.id.ToString(),
                    photo = jobPhoto,
                    caption = message,
                    parse_mode= "HTML",
                    reply_markup = new
                    {
                        inline_keyboard = new[]
                        {
                                new[]
                                {
                                    new
                                    {
                                        text = "WebSite",
                                        url = $"https://uzworks.uz/jobs/{id}"
                                    }
                                }
                            }
                    }
                };

                var jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(sendPhotoUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Response: {responseContent}");
            }
        }

        public static string GenerateTelegramMessage(JobVM model)
        {
            string telegramMessage = "<b>Ish turi:</b> " + model.Title + "\n\n"
                                + "<b>💰 Ish haqi: (Зарплата):</b> " + model.Salary + "\n\n"
                                + "<b>   Gender: (Зарплата):</b> " + model.Gender + "\n\n"
                                + "<b>   Working Time:</b> " + model.WorkingTime + "\n\n"
                                + "<b>   Working Schedule:</b> " + model.WorkingSchedule + "\n\n"
                                + "<b>   Deadline:</b> " + model.Deadline + "\n\n"
                                + "<b>Telegram Link:</b> " + model.TelegramLink + "\n\n"
                                + "<b>Instagram Link:</b> " + model.InstagramLink + "\n\n"
                                + "<b>📞 Phone number:</b> " + model.PhoneNumber + "\n\n";

            return telegramMessage;
        }
    }
}