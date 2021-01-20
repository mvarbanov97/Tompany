using System;
using System.Collections.Generic;
using System.Text;
using Tompany.Data.Common.Models;

namespace Tompany.Data.Models
{
    public class View : BaseModel<string>
    {
        public View()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public Trip Trip { get; set; }

        public string UserId { get; set; }
    }
}
