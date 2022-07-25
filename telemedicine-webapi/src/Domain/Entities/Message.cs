﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Message
    {
        public Guid Id { get; set; }
        public string MessageText { get; set; }
        public DateTime Time => DateTime.UtcNow;
        public Guid SenderUserId { get; set; }
        public string ReceiverUserName { get; set; }
        public User AppUser { get; set; }
    }
}
