using System;
using System.Collections.Generic;
using System.Text;
using Tompany.Data.Models;

namespace Tompany.Web.ViewModels.Chat
{
    public class GroupChatViewModel
    {
        public string GroupName { get; set; }

        public ICollection<ChatMessage> ChatMessages { get; set; } = new HashSet<ChatMessage>();

        public ICollection<ApplicationUser> Users { get; set; } = new HashSet<ApplicationUser>();
    }
}
