using System.Collections.Generic;

namespace WebApplication.Models
{
    public class Message
    {
        public string Subject { get; set; }

        public ItemBody Body { get; set; }

        public List<Recipient> ToRecipients { get; set; }
    }
}
