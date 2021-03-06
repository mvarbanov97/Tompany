﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Tompany.Data.Models;
using Tompany.Services.Mapping;

namespace Tompany.Web.ViewModels.Contacts.InputModels
{
    public class ContactInputModel : IMapFrom<ContactFormEntry>
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Content { get; set; }
    }
}
