namespace WebApplication.Models
{
    public class MessageRequest
    {
        public Message Message { get; set; }

        public bool SaveToSentItems { get; set; }
    }
}
