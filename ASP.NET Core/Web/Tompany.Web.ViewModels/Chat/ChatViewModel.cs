using System;
using System.Collections.Generic;
using System.Text;
using Tompany.Data.Models;

namespace Tompany.Web.ViewModels.Chat
{
    public class ChatViewModel
    {
        public ApplicationUser FromUser { get; set; }

        public ApplicationUser ToUser { get; set; }

        public ICollection<ChatMessage> ChatMessages { get; set; } = new HashSet<ChatMessage>();

        public string GroupName { get; set; }
    }
}
