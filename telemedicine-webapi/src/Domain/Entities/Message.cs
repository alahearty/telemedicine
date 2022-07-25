using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Message : BaseAuditableEntity
    {
        public string MessageText { get; set; }
        public DateTime Time => DateTime.UtcNow;
        public int SenderUserId { get; set; }
        public string ReceiverUserName { get; set; }
        public User AppUser { get; set; }
    }
}
