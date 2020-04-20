using System;
using System.Collections.Generic;
using System.Text;
using Tompany.Data.Common.Models;

namespace Tompany.Data.Models
{
    public class View : BaseModel<string>
    {
        public string UserId { get; set; }
    }
}
