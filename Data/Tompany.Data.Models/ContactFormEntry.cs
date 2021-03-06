﻿using System;
using System.Collections.Generic;
using System.Text;
using Tompany.Data.Common.Models;

namespace Tompany.Data.Models
{
    public class ContactFormEntry : BaseModel<int>
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }
    }
}
