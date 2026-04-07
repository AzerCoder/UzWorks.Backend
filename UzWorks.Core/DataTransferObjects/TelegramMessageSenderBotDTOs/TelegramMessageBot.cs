namespace UzWorks.Core.DataTransferObjects.TelegramMessageSenderBotDTOs
{
    public class ChannelPost
    {
        public long message_id { get; set; }
        public SenderChat sender_chat { get; set; }
        public Chat chat { get; set; }
        public long date { get; set; }
        public string text { get; set; }
    }

    public class Chat
    {
        public long id { get; set; }
        public string first_name { get; set; }
        public string username { get; set; }
        public string type { get; set; }
        public string title { get; set; }
    }

    public class From
    {
        public long id { get; set; }
        public bool is_bot { get; set; }
        public string first_name { get; set; }
        public string username { get; set; }
        public string language_code { get; set; }
    }

    public class Message
    {
        public long message_id { get; set; }
        public From from { get; set; }
        public Chat chat { get; set; }
        public long date { get; set; }
        public string text { get; set; }
    }

    public class MyChatMember
    {
        public Chat chat { get; set; }
        public From from { get; set; }
        public long date { get; set; }
    }

    public class Result
    {
        public long update_id { get; set; }
        public MyChatMember my_chat_member { get; set; }
        public ChannelPost channel_post { get; set; }
    }

    public class Root
    {
        public bool ok { get; set; }
        public List<Result> result { get; set; }
    }

    public class SenderChat
    {
        public long id { get; set; }
        public string title { get; set; }
        public string type { get; set; }
    }

    public class User
    {
        public long id { get; set; }
        public bool is_bot { get; set; }
        public string first_name { get; set; }
        public string username { get; set; }
    }
}
