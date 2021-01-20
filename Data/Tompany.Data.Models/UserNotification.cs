using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Tompany.Data.Models.Enums;

namespace Tompany.Data.Models
{
    public class UserNotification
    {
        public UserNotification()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Key]
        public string Id { get; set; }

        [Required]
        public NotificationType NotificationType { get; set; }

        [Required]
        public NotificationStatus Status { get; set; }

        [Required]
        [ForeignKey(nameof(ApplicationUser))]
        public string ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        [MaxLength(20)]
        public string TargetUsername { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public string Link { get; set; }

        [Required]
        public string Text { get; set; }
    }
}
